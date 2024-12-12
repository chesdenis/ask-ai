using System.Text.RegularExpressions;
using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Model;
using AITextWriter.Services.Abstractions;

namespace AITextWriter.Services;

public class UserPromptReader(
    IFileSystemProvider fileSystemProvider,
    IParametersProvider parametersProvider) : IUserPromptReader
{
    private const string ApiKeyFileName = "apikey";
    private const string PromptSelectPattern = @"\n{3,}";

    public async Task<string[]> GetTagsAsync()
    {
        var workingPath = await parametersProvider.GetWorkingFolderPathAsync();
        var files = await fileSystemProvider.GetFilePathsAsync(
            workingPath,
            "*.", false);

        var tags = files.Select(Path.GetFileNameWithoutExtension).ToList();

        var workingFilePath = await parametersProvider.GetWorkingFilePathAsync();

        tags.Remove(Path.GetFileNameWithoutExtension(workingFilePath)); // this is context file

        // these are generator file if available (on MacOs/Linux can be)
        tags.Remove("AITextWriterListen");
        tags.Remove("AITextWriterProcess");
        tags.Remove("AITextWriterSummarize");
        tags.Remove(ApiKeyFileName);

        return tags.ToArray()!;
    }

    public async Task<string> GetApiKeyAsync()
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

    public async Task<Prompt[]> GetPromptsAsync()
    {
        var workingFile = await parametersProvider.GetWorkingFilePathAsync();
        var contents = await fileSystemProvider.ReadAllTextAsync(workingFile);

        var contentParts = Regex.Split(contents, PromptSelectPattern);
        
        // List to hold the extracted roles and text
        List<(string Role, string Text)> extractedTexts = new();

        foreach (var part in contentParts)
        {
            if (string.IsNullOrWhiteSpace(part))
            {
                continue;
            }
            
            var role = "user";
            var text = part;
            extractedTexts.Add((role, text));
        }

        return extractedTexts.Select(s => new Prompt
        {
            role = s.Role,
            content = s.Text
        }).ToArray();
    }
}