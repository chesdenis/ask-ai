using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Infrastructure.Options;

namespace AITextWriter.Infrastructure;

public class FileSystemContextParameters(IListenContextOptions options)
    : IFileSystemContextParameters
{
    public Task<string> GetWorkingFolderPathAsync() => Task.FromResult(options.WorkingFolder);
}