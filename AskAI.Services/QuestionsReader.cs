using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Abstractions;

namespace AskAI.Services;

public class QuestionsReader(
    IFileSystemProvider fileSystemProvider) : IQuestionsReader
{
    private const string PromptSelectPattern = "---";

    public async IAsyncEnumerable<Prompt> ReadAsync(string filePath)
    {
        var contents = await fileSystemProvider.ReadAllTextAsync(filePath);

        var contentParts = new Regex(PromptSelectPattern).Split(contents);

        foreach (var part in contentParts)
        {
            if (string.IsNullOrWhiteSpace(part))
            {
                continue;
            }

            var role = ReservedKeywords.User;
            var text = part;

            yield return new Prompt
            {
                role = role,
                content = text
            };
        }
    }

}