using System.Text.RegularExpressions;
using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Model;

namespace AITextWriter.Infrastructure;

public class TextModelContextProvider(
    IFileSystemProvider fileSystemProvider,
    IParametersProvider parametersProvider) : ITextModelContextProvider
{
    private const string ApiKeyFileName = "apikey";
    
    // Regular expression to match sections starting with ### followed by role and the text
    private const string PromptRolesSectionsPattern =   @"### (user|assistant)\n([\s\S]*?)(?=(### (user|assistant)|$))";
    private const string PromptRolesSectionsPatternV2 = @"###\s*(user|assistant)\s*\n([\s\S]*?)(?=(###\s*(user|assistant)|$))";

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

    public async Task<Prompt[]> GetPrompts()
    {
        var workingFile = await parametersProvider.GetWorkingFilePathAsync();
        var contents = await fileSystemProvider.ReadAllTextAsync(workingFile);

        // case when there is no role specified
        if (!contents.Trim().StartsWith("###"))
        {
            return
            [
                new Prompt
                {
                    role = "user",
                    content = contents
                }
            ];
        }
        
        // List to hold the extracted roles and text
        List<(string Role, string Text)> extractedTexts = new();

        foreach (Match match in Regex.Matches(contents, PromptRolesSectionsPatternV2, RegexOptions.IgnoreCase))
        {
            var role = match.Groups[1].Value.Trim();
            var text = match.Groups[2].Value.Trim();
            extractedTexts.Add((role, text));
        }

        return extractedTexts.Select(s => new Prompt
        {
            role = s.Role,
            content = s.Text
        }).ToArray();
    }
}