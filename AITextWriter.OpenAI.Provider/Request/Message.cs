using Newtonsoft.Json;

namespace AITextWriter.OpenAI.Provider.Request;

[JsonObject(MemberSerialization.OptIn)]
public class Message
{
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}