namespace AskAI.Model;

public record PromptLink
{
    public required string Source { get; set; }
    public required string Type { get; set; }
}