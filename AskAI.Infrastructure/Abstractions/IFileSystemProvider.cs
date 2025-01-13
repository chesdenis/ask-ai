namespace AskAI.Infrastructure.Abstractions;

public interface IFileSystemProvider
{
    Task<string> ReadAllTextAsync(string path);
    Task WriteAllTextAsync(string path, string content);
    Task<string[]> GetFilePathsAsync(string parentFolder, string pattern, bool recursive);
    IEnumerable<string> EnumerateFiles(IEnumerable<string> linksOfFilesOrDirs);
    string CalculateBaseDirectory(IEnumerable<string> paths);
    string EncodeAsBase64(string filePathToEncode);
    bool FileExist(string filePath);
}