namespace AskAI.Infrastructure.Abstractions;

public interface IFileSystemLinksCollector
{
    IEnumerable<string> Collect(string contents);
}