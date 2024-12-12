using System.Text.RegularExpressions;
using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Model;
using AITextWriter.Services.Abstractions;
using AITextWriter.Services.Extensions;

namespace AITextWriter.Services;

public class AssistantPromptReader(
        IFileSystemProvider fileSystemProvider,
        IParametersProvider parametersProvider
        ): IAssistantPromptReader
{
    // Regular expression to match sections starting with ### followed by role and the text
    private const string PromptSelectPattern = @"###\s*(user|assistant)\s*\n([\s\S]*?)(?=(###\s*(user|assistant)|$))";

    public async Task<Prompt[]> GetPromptsAsync()
    { 
        var workingFile = await parametersProvider.GetWorkingFilePathAsync();
        var answerFile = await workingFile.GetAnswerFilePathAsync();

        var contents = await fileSystemProvider.ReadAllTextAsync(answerFile);
        
        // List to hold the extracted roles and text
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

        foreach (Match match in Regex.Matches(contents, PromptSelectPattern, RegexOptions.IgnoreCase))
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