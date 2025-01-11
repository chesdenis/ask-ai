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
    public async Task RunAsync(string workingFilePath, CancellationToken ct)
    {
        var documentContent = await File.ReadAllTextAsync(workingFilePath, ct);

        logger.LogInformation("Incoming file: {WorkingFilePath}", workingFilePath);

        var questionPrompts = await questionsReader.ReadAsync(documentContent).ToListAsync(cancellationToken: ct);
        var enrichedPrompts = await promptEnricher.EnrichAsync(questionPrompts.ToArray(), workingFilePath);
        var existedConversation = await conversationReader.EnumerateConversationPairsAsync(workingFilePath)
            .ToListAsync(cancellationToken: ct);

        var conversationPairs = BuildConversationPairs(existedConversation, enrichedPrompts);

        await ProcessAssistantStep(workingFilePath, conversationPairs);
        
        logger.LogInformation("Process finished for file: {WorkingFilePath}", workingFilePath);
    }

    private async Task ProcessAssistantStep(string workingFilePath, List<ConversationPair> conversationPairs)
    {
        var baseDir = AppContext.BaseDirectory;

        var apiRequestSettings = new ApiRequestSettings
        {
            ApiKey = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.ApiKey),
            Model = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.Model),
            Endpoint = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.Endpoint),
            TimeoutMinutes = ContextExtensions.ResolveRequiredKey<int>(baseDir, ReservedKeywords.TimeoutMinutes),
        };
        
        if (conversationPairs.Count == 0)
        {
            logger.LogInformation("No prompts to ask.");
            return;
        }

        if (conversationPairs.Last().AssistantAnswer != null)
        {
            logger.LogInformation("Last question was answered.");
            return;
        }

        var assistantAnswer = await
            assistantResponseProvider.GetAssistantAnswer(conversationPairs.ToPrompts().ToArray(),
                apiRequestSettings);

        conversationPairs.Last().AssistantAnswer = new Prompt
        {
            role = ReservedKeywords.Assistant,
            content = assistantAnswer
        };

        await assistantAnswersWriter
            .WriteConversationAsync(
                conversationPairs.ToPrompts().ToArray(),
                workingFilePath);
    }

    private List<ConversationPair> BuildConversationPairs(
        List<ConversationPair> existedConversation, 
        Prompt[] enrichedPrompts)
    {
        var existedConversationsWithHashes = existedConversation.Select(s => new
        {
            s.UserQuestion,
            UserQuestionHash = s.UserQuestion.ToStringHash(),
            s.AssistantAnswer,
            AssistantAnswerHash = s.AssistantAnswer?.ToStringHash()
        }).ToList();

        var conversationPairs = new List<ConversationPair>();

        foreach (var prompt in enrichedPrompts)
        {
            var userQuestionHash = prompt.ToStringHash();

            var existedPair = existedConversationsWithHashes
                .FirstOrDefault(w => w.UserQuestionHash == userQuestionHash);

            if (existedPair != null)
            {
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
            }
        }

        return conversationPairs;
    }
}