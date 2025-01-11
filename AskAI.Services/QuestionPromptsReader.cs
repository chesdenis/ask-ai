using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Abstractions;

namespace AskAI.Services;

public partial class QuestionPromptsReader(
    IFileSystemProvider fileSystemProvider) : IQuestionPromptsReader
{
    private const string PromptSelectPattern = "---";
    
    public async Task<Prompt[]> ReadAsync(string filePath)
    {
        var contents = await fileSystemProvider.ReadAllTextAsync(filePath);

        var contentParts = QuestionPromptRegex().Split(contents);
        
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

    [GeneratedRegex(PromptSelectPattern)]
    private static partial Regex QuestionPromptRegex();
}