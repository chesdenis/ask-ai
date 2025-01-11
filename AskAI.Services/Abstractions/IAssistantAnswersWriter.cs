using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IAssistantAnswersWriter
{
    Task WriteConversationAsync(IEnumerable<Prompt> prompts, string workingDocument);
}