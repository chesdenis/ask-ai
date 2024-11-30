using AITextWriter.Infrastructure.Abstractions;

namespace AITextWriter.Infrastructure;

public class TextModelContextProvider(
    IFileSystemProvider fileSystemProvider,
    IParametersProvider parametersProvider) : ITextModelContextProvider
{
    private const string ApiKeyFileName = "apikey";

    public async Task<string[]> GetTagsAsync()
    {
        var workingPath = await parametersProvider.GetWorkingFolderPathAsync();
        var files = await fileSystemProvider.GetFilePathsAsync(
            workingPath,
            "*.", false);

        var tags = files.Select(Path.GetFileNameWithoutExtension).ToList();

        var workingFilePathAsync = await parametersProvider.GetWorkingFilePathAsync();

        tags.Remove(Path.GetFileNameWithoutExtension(workingFilePathAsync)); // this is context file

        // these are generator file if available (on MacOs/Linux can be)
        tags.Remove("AITextWriterListen");
        tags.Remove("AITextWriterProcess");
        tags.Remove("AITextWriterSummarize");
        tags.Remove(ApiKeyFileName);

        return tags.ToArray()!;
    }

    public async Task<string> GetApiKey()
    {
        var workingFolder = await parametersProvider.GetWorkingFolderPathAsync();

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