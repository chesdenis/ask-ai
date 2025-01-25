namespace AskAI.Model;

public record FileSystemLink
{
    public required string Key { get; set; }
    public required string Path { get; set; }
}