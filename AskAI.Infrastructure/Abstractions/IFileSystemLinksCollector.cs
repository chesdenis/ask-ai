namespace AskAI.Infrastructure.Abstractions;

public interface IFileSystemLinksCollector
{
    IEnumerable<KeyValuePair<string, string>> Collect(string contents);
}