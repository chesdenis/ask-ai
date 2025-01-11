using AskAI.Infrastructure.Abstractions;
using AskAI.Infrastructure.Options;

namespace AskAI.Infrastructure;

public class WorkSpaceContext(IWatchOptions folderOptions, IFileSystemProvider fileSystemProvider)
    : IWorkSpaceContext
{
    private const string ApiKeyFileName = "apikey";
    
    public Task<string> GetWorkingFolderPathAsync() => Task.FromResult(folderOptions.WorkingFolder);
    
    public async Task<string[]> GetTagsAsync(string filePath)
    {
        var workingPath = await GetWorkingFolderPathAsync();
        var files = await fileSystemProvider.GetFilePathsAsync(
            workingPath,
            "*.", false);

        var tags = files.Select(Path.GetFileNameWithoutExtension).ToList();
        
        // this is target file, and we need to exclude it from tags
        tags.Remove(Path.GetFileNameWithoutExtension(filePath)); 

        // these are generator file if available (on MacOs/Linux can be)
        tags.Remove("AskAI");
        tags.Remove(ApiKeyFileName);

        return tags.ToArray()!;
    }
    
    public async Task<string> GetApiKeyAsync()
    {
        var workingFolder = await GetWorkingFolderPathAsync();

        var apiKeyPathSearchResults = await fileSystemProvider.GetFilePathsAsync(
            workingFolder,
            ApiKeyFileName,
            false);

        var path = apiKeyPathSearchResults.FirstOrDefault();

        if (path == null)
        {
            throw new ArgumentException($"Unable to find api key. Tried search here {workingFolder}",
                nameof(workingFolder));
        }
        
        var apiKey = await fileSystemProvider.ReadAllTextAsync(path);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException(
                $"Api key is empty. Please provide a valid api key. Tried search here {workingFolder}");
        }

        return apiKey;
    }
}