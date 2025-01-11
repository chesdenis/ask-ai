using AskAI.Model;

namespace AskAI.Infrastructure.Abstractions;

public interface IAssistantResponseProvider
{
    Task<string> GetAssistantAnswer(
        Prompt[] prompts,
        ApiRequestSettings requestSettings);
}