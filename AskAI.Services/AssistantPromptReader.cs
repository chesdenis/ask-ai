using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Extensions;
using AskAI.Services.Abstractions;

namespace AskAI.Services;

public class AssistantPromptReader(
        IFileSystemProvider fileSystemProvider
        ): IAssistantPromptReader
{
    // Regular expression to match sections starting with ### followed by role and the text
    private const string PromptSelectPattern = @"###\s*(user|assistant)\s*\n([\s\S]*?)(?=(###\s*(user|assistant)|$))";

    public async Task<Prompt[]> GetPromptsAsync(string filePath)
    { 
        var answerFile = await filePath.GetAnswerFilePathAsync();

        if (!fileSystemProvider.FileExist(answerFile))
        {
            return [];
        }

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