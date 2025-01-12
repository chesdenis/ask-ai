using AskAI.Model;

namespace AskAI.OpenAI.Provider.Request;

public class AiRequest
{
    public string model { get; set; }

    public AiPromptRequest[] messages { get; set; }
}

public class AiPromptRequest
{
    public required string role { get; set; }

    public required AiPromptEntryRequest[] content { get; set; }
}

public class AiPromptEntryRequest : Dictionary<string, string>;