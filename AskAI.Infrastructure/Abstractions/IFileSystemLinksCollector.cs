using AskAI.Model;

namespace AskAI.Infrastructure.Abstractions;

public interface IFileSystemLinksCollector
{
    IEnumerable<FileSystemLink> Collect(string contents);
}