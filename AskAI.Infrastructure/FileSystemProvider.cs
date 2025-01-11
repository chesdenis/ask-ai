using AskAI.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;

namespace AskAI.Infrastructure;

public class FileSystemProvider(ILogger<FileSystemProvider> logger) : IFileSystemProvider
{
    private readonly ILogger _logger = logger;

    public async Task<string> ReadAllTextAsync(string path)
    {
        try
        {
            _logger.LogDebug("Reading all text from {Path}", path);
            var content = await File.ReadAllTextAsync(path);
            _logger.LogDebug("Successfully read text from {Path}", path);
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading text from {Path}", path);
            throw;
        }
    }

    public async Task WriteAllTextAsync(string path, string content)
    {
        try
        {
            _logger.LogDebug("Writing text to {Path}", path);
            await File.WriteAllTextAsync(path, content);
            _logger.LogDebug("Successfully wrote text to {Path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing text to {Path}", path);
            throw;
        }
    }

    public async Task<string[]> GetFilePathsAsync(string parentFolder, string pattern, bool recursive)
    {
        try
        {
            _logger.LogDebug("Getting file paths from {ParentFolder} with pattern {Pattern} and recursive {Recursive}", parentFolder, pattern, recursive);
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var filePaths = Directory.GetFiles(parentFolder, pattern, searchOption);
            var filteredPaths = new List<string>();
            foreach (var filePath in filePaths)
            {
                try
                {
                    var fi = new FileInfo(filePath);
                    if (fi.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    logger.LogWarning("Unable to read file attributes for {FilePath}", filePath);
                    continue;
                }
                
                filteredPaths.Add(filePath);
            }
            
            _logger.LogDebug("Successfully retrieved file paths from {ParentFolder}", parentFolder);
            return await Task.FromResult(filteredPaths.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file paths from {ParentFolder}", parentFolder);
            throw;
        }
    }

    public bool FileExist(string filePath)
    {
        try
        {
            return File.Exists(filePath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error checking if file exists {FilePath}", filePath);
            throw;
        }
    }
}