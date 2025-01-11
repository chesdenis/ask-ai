using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Extensions;
using Microsoft.Extensions.Logging;

namespace AskAI.Services.Apps;

public class AskAiConsoleMode(IAssistantResponseProvider assistantResponseProvider, ILogger<AskAiConsoleMode> logger)
{
    public async Task RunAsync(CancellationToken ct)
    {
        var baseDir = AppContext.BaseDirectory;

        Console.WriteLine("Hello! Console mode is running. Base directory: {0}", baseDir);
        Console.WriteLine("What would you like to ask?");

        var conversation = new List<ConversationPair>();

        while (true)
        {
            if (ct.IsCancellationRequested)
            {
                break;
            }
            
            var questionText = Console.ReadLine();
            var apiRequestSettings = new ApiRequestSettings
            {
                ApiKey = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.ApiKey),
                Model = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.Model),
                Endpoint = ContextExtensions.ResolveRequiredKey<string>(baseDir, ReservedKeywords.Endpoint),
                TimeoutMinutes = ContextExtensions.ResolveRequiredKey<int>(baseDir, ReservedKeywords.TimeoutMinutes),
            };

            if (string.IsNullOrWhiteSpace(questionText))
            {
                continue;
            }

            var promptsToSend = conversation.ToPrompts().ToList();
            promptsToSend.Add(new Prompt()
            {
                role = ReservedKeywords.User,
                content = questionText
            });

            var answerText = await assistantResponseProvider.GetAssistantAnswer(
                promptsToSend.ToArray(), apiRequestSettings
            )!.TryWithFallbackValueAs(null);

            if (answerText != null)
            {
                logger.LogInformation(answerText);
                
                conversation.Add(new ConversationPair()
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
                });
            }
        }

        Console.WriteLine("Console mode terminated");
    }
}