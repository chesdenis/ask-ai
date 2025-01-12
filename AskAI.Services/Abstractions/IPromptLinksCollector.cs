namespace AskAI.Services.Abstractions;

public interface IPromptLinksCollector
{
    IEnumerable<string> Collect(string contents);
}