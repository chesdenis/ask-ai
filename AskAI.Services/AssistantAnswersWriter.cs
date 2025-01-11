using System.Text;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Abstractions;
using AskAI.Services.Extensions;

namespace AskAI.Services;

public class AssistantAnswersWriter(IFileSystemProvider fileSystemProvider) : IAssistantAnswersWriter
{
    public async Task WriteConversationAsync(IEnumerable<Prompt> prompts, string workingDocument)
    {
        var sb = new StringBuilder();

        foreach (var prompt in prompts)
        {
            sb.AppendLine($"### {prompt.role}");
            sb.AppendLine(prompt.content);
            sb.AppendLine();
        }

        var conversationFilePath = await workingDocument.GetConversationFilePathAsync();

        await fileSystemProvider.WriteAllTextAsync(conversationFilePath, sb.ToString());
    }
}