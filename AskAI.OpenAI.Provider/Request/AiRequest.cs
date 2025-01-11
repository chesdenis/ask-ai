using AskAI.Model;

namespace AskAI.OpenAI.Provider.Request;

public class AiRequest
{
    public string model { get; set; }

    public Prompt[] messages { get; set; }
}