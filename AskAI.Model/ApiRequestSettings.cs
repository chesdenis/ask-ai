namespace AskAI.Model;

public record ApiRequestSettings
{
    public required string ApiKey { get; set; }
    public required string Model { get; set; }
    public required string Endpoint { get; set; }
    public required double TimeoutMinutes { get; set; }
}