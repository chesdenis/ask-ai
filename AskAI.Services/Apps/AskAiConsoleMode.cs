using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Abstractions;
using AskAI.Services.Extensions;

namespace AskAI.Services.Apps;

public class AskAiConsoleMode(
    IAssistantResponseProvider assistantResponseProvider,
    IAssistantAnswersWriter assistantAnswersWriter)
{
    public async Task RunAsync(CancellationToken ct)
    {
        var baseDir = AppContext.BaseDirectory;

        var apiRequestSettings = new ApiRequestSettings
        {
            ApiKey = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.ApiKey),
            Model = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.Model),
            Endpoint = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.Endpoint),
            TimeoutMinutes = ContextExtensions.ResolveRequiredKey<int>(baseDir, ReservedKeywords.TimeoutMinutes),
        };

        Console.WriteLine(":) -> ask here : {0}", baseDir);
        var questionText = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(questionText))
        {
            Console.WriteLine(" :((( -> no question asked");
            return;
        }

        var answerText = await assistantResponseProvider.GetAssistantAnswer(
            [
                new Prompt()
                {
                    role = ReservedKeywords.User,
                    content = questionText
                }
            ], apiRequestSettings
        )!.TryWithFallbackValueAs(null);

        var fileNameToStore = await assistantResponseProvider.GetAssistantAnswer(
        [
            new Prompt()
            {
                role = ReservedKeywords.User,
                content = answerText + Environment.NewLine +
                          ". Based on that answer, give me file name to store this conversation as md file. But as output just show me file name without any text"
            }
        ], apiRequestSettings);

        if (answerText != null)
        {
            fileNameToStore = fileNameToStore
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("\"", "");
            
            Console.WriteLine($"Answer: {answerText}, File: {fileNameToStore}");

            var conversationPair = new ConversationPair()
            {
                UserQuestion = new Prompt
                {
                    role = ReservedKeywords.User,
                    content = questionText
                },
                AssistantAnswer = new Prompt
                {
                    role = ReservedKeywords.Assistant,
                    content = answerText
                }
            };

            var workingFilePath = Path.Combine(baseDir, fileNameToStore);

            var prompts = new[] { conversationPair }.ToPrompts().ToArray();

            await assistantAnswersWriter.WriteQuestionAsync(questionText, workingFilePath);
            
            await assistantAnswersWriter
                .WriteConversationAsync(
                    prompts,
                    workingFilePath);

            await assistantAnswersWriter.WriteAnswerAsync(
                prompts,
                workingFilePath);
        }
    }
}