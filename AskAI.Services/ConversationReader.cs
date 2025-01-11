using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Extensions;
using AskAI.Services.Abstractions;

namespace AskAI.Services;

public class ConversationReader(
    IFileSystemProvider fileSystemProvider
) : IConversationReader
{
    // Regular expression to match sections starting with ### followed by role and the text
    private const string PromptSelectPattern = @"###\s*(user|assistant)\s*\n([\s\S]*?)(?=(###\s*(user|assistant)|$))";

    public async IAsyncEnumerable<ConversationPair> EnumerateAsync(string filePath)
    {
        var answerFile = await filePath.GetAnswerFilePathAsync();

        if (!fileSystemProvider.FileExist(answerFile))
        {
            yield break;
        }

        var contents = await fileSystemProvider.ReadAllTextAsync(answerFile);

        // List to hold the extracted roles and text
        // case when there is no role specified
        if (!contents.Trim().StartsWith("###"))
        {
            yield return new ConversationPair()
            {
                UserQuestion = new Prompt
                {
                    role = "user",
                    content = contents
                }
            };
            
            yield break;
        }

        // List to hold the extracted roles and text
        List<(string Role, string Text)> extractedTexts = new();

        foreach (Match match in Regex.Matches(contents, PromptSelectPattern, RegexOptions.IgnoreCase))
        {
            var role = match.Groups[1].Value.Trim();
            var text = match.Groups[2].Value.Trim();
            extractedTexts.Add((role, text));
        }

        for (int i = 0; i < extractedTexts.Count; i+=2)
        {
            yield return new ConversationPair
            {
                UserQuestion = new Prompt
                {
                    role = extractedTexts[i].Role,
                    content = extractedTexts[i].Text    
                },
                AssistantAnswer = new Prompt
                {
                    role = extractedTexts[i + 1].Role,  
                    content = extractedTexts[i + 1].Text
                }
            };
        }
    }
}