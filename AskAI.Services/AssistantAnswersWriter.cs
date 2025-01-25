using System.Text;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Abstractions;
using AskAI.Services.Extensions;

namespace AskAI.Services;

public class AssistantAnswersWriter(IFileSystemProvider fileSystemProvider) : IAssistantAnswersWriter
{
    public async Task WriteQuestionAsync(string question, string workingDocument)
    {
        await fileSystemProvider.WriteAllTextAsync(workingDocument, question);
    }

    public async Task WriteConversationAsync(IEnumerable<Prompt> prompts, string workingDocument)
    {
        var sb = new StringBuilder();

        foreach (var prompt in prompts)
        {
            sb.AppendLine($"### {prompt.role}");
            sb.AppendLine(prompt.content);
            sb.AppendLine();
        }

        var outputFilePath = await workingDocument.GetConversationFilePathAsync();

        await fileSystemProvider.WriteAllTextAsync(outputFilePath, sb.ToString());
    }

    public async Task WriteAnswerAsync(IEnumerable<Prompt> prompts, string workingDocument)
    {
        var sb = new StringBuilder();

        var lastPrompt = prompts.LastOrDefault();
        if (lastPrompt != null)
        {
            sb.AppendLine(lastPrompt.content);
            sb.AppendLine();
        }

        var outputFilePath = await workingDocument.GetAnswerFilePathAsync();

        await fileSystemProvider.WriteAllTextAsync(outputFilePath, sb.ToString());
    }
}