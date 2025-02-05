using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Abstractions;
using AskAI.Services.Extensions;
using Microsoft.Extensions.Logging;

namespace AskAI.Services.Apps;
 
public class AskAiDocumentMode(
    IAssistantResponseProvider assistantResponseProvider,
    IQuestionsReader questionsReader,
    IPromptEnricher promptEnricher,
    IConversationReader conversationReader,
    IAssistantAnswersWriter assistantAnswersWriter,
    ILogger<AskAiDocumentMode> logger)
{
    public async Task RunAsync(string workingFilePath, string languageModel, CancellationToken ct)
    {
        logger.LogInformation("Incoming file: {WorkingFilePath}", workingFilePath);

        var questionPrompts = await questionsReader.ReadAsync(workingFilePath).ToListAsync(cancellationToken: ct);
        var enrichedPrompts = await promptEnricher.EnrichAsync(questionPrompts.ToArray(), workingFilePath);
        var existedConversation = await conversationReader.EnumerateConversationPairsAsync(workingFilePath)
            .ToListAsync(cancellationToken: ct);

        var conversationPairs = BuildConversationPairs(existedConversation, enrichedPrompts);

        await ProcessAssistantStep(workingFilePath, languageModel, conversationPairs);
        
        logger.LogInformation("Process finished for file: {WorkingFilePath}", workingFilePath);
    }

    private async Task ProcessAssistantStep(string workingFilePath, string languageModel, List<ConversationPair> conversationPairs)
    {
        var baseDir = Path.GetDirectoryName(workingFilePath);

        var apiRequestSettings = new ApiRequestSettings
        {
            ApiKey = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.ApiKey),
            Model = languageModel,
            Endpoint = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.Endpoint),
            TimeoutMinutes = ContextExtensions.ResolveRequiredKey<int>(baseDir, ReservedKeywords.TimeoutMinutes),
        };
        
        if (conversationPairs.Count == 0)
        {
            logger.LogInformation("No prompts to ask.");
            return;
        }
        
        // here we need to reconstruct answers and question tree
        // if something was changed in conversation backbone we need to reflect this.
        for (var pairIndex = 0; pairIndex < conversationPairs.Count; pairIndex++)
        {
            var pair = conversationPairs[pairIndex];
            if (pair.AssistantAnswer != null)
            {
                logger.LogInformation("This question {question} was answered.", pair.UserQuestionHash);
                continue;
            }
            
            var conversationPairsToAsk = conversationPairs.Take(pairIndex + 1).ToPrompts().ToArray();

            var assistantAnswer = await
                assistantResponseProvider.GetAssistantAnswer(conversationPairsToAsk,
                    apiRequestSettings);
            
            pair.AssistantAnswer = new Prompt
            {
                role = ReservedKeywords.Assistant,
                content = assistantAnswer
            };
            
            await assistantAnswersWriter
                .WriteConversationAsync(
                    conversationPairs.ToPrompts().ToArray(),
                    workingFilePath);
        }

        await assistantAnswersWriter.WriteAnswerAsync(
            conversationPairs.ToPrompts().ToArray(), 
            workingFilePath);
    }

    private List<ConversationPair> BuildConversationPairs(
        List<ConversationPair> existedConversation, 
        Prompt[] enrichedPrompts)
    {
        // this is to not calculate hash multiple times during next loops
        var existedConversationsWithHashes = existedConversation.Select(s => new
        {
            s.UserQuestion,
            s.UserQuestionHash,
            s.AssistantAnswer,
            s.AssistantAnswerHash
        }).ToList();

        var conversationPairs = new List<ConversationPair>();

        bool foundFirstMissingAnswers = false;
        foreach (var prompt in enrichedPrompts)
        {
            // this is because in case if we spot something which changes the conversation tree
            // we need to reflect this in the conversation tree
            // we need to assign null to assistant answers for next questions
            if (foundFirstMissingAnswers)
            {
                conversationPairs.Add(new ConversationPair
                {
                    UserQuestion = prompt,
                    AssistantAnswer = null
                });
                continue;
            }
            
            var userQuestionHash = prompt.ToStringHash();

            var existedPair = existedConversationsWithHashes
                .FirstOrDefault(w => w.UserQuestionHash == userQuestionHash);

            if (existedPair != null)
            {
                logger.LogInformation("Found existing answer for question: {UserQuestion}", existedPair.UserQuestion);
                conversationPairs.Add(new ConversationPair
                {
                    UserQuestion = existedPair.UserQuestion,
                    AssistantAnswer = existedPair.AssistantAnswer
                });
            }
            else
            {
                conversationPairs.Add(new ConversationPair
                {
                    UserQuestion = prompt,
                    AssistantAnswer = null
                });

                foundFirstMissingAnswers = true;
            }
        }

        return conversationPairs;
    }
}