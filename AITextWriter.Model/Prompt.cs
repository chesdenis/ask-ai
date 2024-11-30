using Newtonsoft.Json;

namespace AITextWriter.Model;

[JsonObject(MemberSerialization.OptIn)]
public class Prompt
{
    public string role { get; set; }

    public string content { get; set; }
}