using Newtonsoft.Json;

namespace AskAI.Model;

[JsonObject(MemberSerialization.OptIn)]
public record Prompt
{
    public required string role { get; set; }

    public required string content { get; set; }

    public override string ToString()
    {
        return $"{nameof(role)}: {role}, {nameof(content)}: {content}";
    }
}