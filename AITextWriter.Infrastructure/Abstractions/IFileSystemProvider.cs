namespace AITextWriter.Infrastructure.Abstractions;

public interface IFileSystemProvider
{
    Task<string> ReadAllTextAsync(string path);
    Task WriteAllTextAsync(string path, string content);
    Task<string[]> GetFilePathsAsync(string parentFolder, string pattern, bool recursive);
    bool FileExist(string filePath);
}