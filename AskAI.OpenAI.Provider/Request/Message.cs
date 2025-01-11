using Newtonsoft.Json;

namespace AskAI.OpenAI.Provider.Request;

[JsonObject(MemberSerialization.OptIn)]
public class Message
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}