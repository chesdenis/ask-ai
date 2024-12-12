using AITextWriter.Model;

namespace AITextWriter.Infrastructure.Abstractions;

public interface IAssistantResponseProvider
{
    Task<string> GetAssistantAnswer(
        Prompt[] prompts,
        ModelDetails modelDetails,
        ApiRequestSettings requestSettings);
}