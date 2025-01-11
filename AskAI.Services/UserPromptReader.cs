using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Abstractions;

namespace AskAI.Services;

public class UserPromptReader(
    IFileSystemProvider fileSystemProvider,
    IWorkingContextParameters workingContextParameters) : IUserPromptReader
{
    private const string ApiKeyFileName = "apikey";
    private const string PromptSelectPattern = @"\n{3,}";

    public async Task<string[]> GetTagsAsync(string filePath)
    {
        var workingPath = await workingContextParameters.GetWorkingFolderPathAsync();
        var files = await fileSystemProvider.GetFilePathsAsync(
            workingPath,
            "*.", false);

        var tags = files.Select(Path.GetFileNameWithoutExtension).ToList();
        
        // this is target file, and we need to exclude it from tags
        tags.Remove(Path.GetFileNameWithoutExtension(filePath)); 

        // these are generator file if available (on MacOs/Linux can be)
        tags.Remove("AITextWriterListen");
        tags.Remove("AskAI");
        tags.Remove("AITextWriterSummarize");
        tags.Remove(ApiKeyFileName);

        return tags.ToArray()!;
    }

    public async Task<string> GetApiKeyAsync()
    {
        var workingFolder = await workingContextParameters.GetWorkingFolderPathAsync();

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

    public async Task<Prompt[]> GetPromptsAsync(string filePath)
    {
        var contents = await fileSystemProvider.ReadAllTextAsync(filePath);

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