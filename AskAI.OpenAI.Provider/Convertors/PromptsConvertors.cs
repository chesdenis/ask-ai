using AskAI.Model;
using AskAI.OpenAI.Provider.Request;

namespace AskAI.OpenAI.Provider.Convertors;

public static class PromptsConvertors
{
    public static IEnumerable<AiPromptRequest> ToAiEntryRequest(this IEnumerable<Prompt> prompts)
    {
        return prompts.Select(p => new AiPromptRequest
        {
            role = p.role,
            content =
            [
                new AiPromptEntryRequest()
                {
                    { "type", "text" },
                    { "text", p.content }
                }
            ]
        });
    }
}