using Newtonsoft.Json;

namespace AskAI.OpenAI.Provider.Response;

[JsonObject(MemberSerialization.OptIn)]
public class Usage
{
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
}