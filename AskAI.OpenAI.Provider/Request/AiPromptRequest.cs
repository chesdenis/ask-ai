namespace AskAI.OpenAI.Provider.Request;

public class AiPromptRequest
{
    public required string role { get; set; }

    public required AiPromptEntryRequest[] content { get; set; }
}