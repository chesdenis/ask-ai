using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IAssistantAnswersWriter
{
    Task WriteConversationAsync(IEnumerable<Prompt> prompts, string workingDocument);
    Task WriteAnswerAsync(IEnumerable<Prompt> prompts, string workingDocument);
    Task WriteSummaryAsync(string summary, string workingDocument);
}