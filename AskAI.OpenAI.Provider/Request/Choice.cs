using Newtonsoft.Json;

namespace AskAI.OpenAI.Provider.Request;

[JsonObject(MemberSerialization.OptIn)]
public class Choice
{
    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("message")]
    public Message Message { get; set; }

    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
}