using AITextWriter.Model;

namespace AITextWriter.Infrastructure.Abstractions;

public interface IAssistantAnswerProvider
{
    Task<string> GetAssistantAnswer(
        Prompt[] prompts,
        ModelDetails modelDetails,
        ApiRequestSettings requestSettings);
}