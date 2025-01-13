using AskAI.Model;
using AskAI.OpenAI.Provider.Request;

namespace AskAI.OpenAI.Provider.Abstraction;

public interface IOpenAiPromptsConvertors
{
    IEnumerable<AiPromptRequest> ToAiEntryRequest(IEnumerable<Prompt> prompts);
}