using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IAssistantAnswersWriter
{
    Task WriteQuestionAsync(string question, string workingDocument);
    Task WriteConversationAsync(IEnumerable<Prompt> prompts, string workingDocument);
    Task WriteAnswerAsync(IEnumerable<Prompt> prompts, string workingDocument);
}