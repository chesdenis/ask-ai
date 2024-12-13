namespace AITextWriter.Infrastructure.Abstractions;

public interface IFileSystemContextParameters
{
    Task<string> GetWorkingFolderPathAsync();
}