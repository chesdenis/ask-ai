using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services.Abstractions;

namespace AskAI.Services;

public class PromptEnricher(IWorkSpaceContext workSpaceContext) : IPromptEnricher
{
    public async Task<Prompt[]> EnrichAsync(Prompt[] input, string filePath)
    {
        var result = await UpdateTags(input, filePath);

        return result;
    }

    private async Task<Prompt[]> UpdateTags(Prompt[] input, string filePath)
    {
        if (!input.Any())
        {
            return input;
        }
        
        var firstPrompt = input.First();
        var tags = await workSpaceContext.GetTagsAsync(filePath);
        if (tags.Length == 0)
        {
            return input;
        }

        var tagsAsString = string.Join(", ", tags);

        var pattern = @"Please use this context: \[.*?\]\.";
        var replacement =
            $"Please use this context: {tagsAsString}."; // Replace with the fragment you want to insert

        var result = Regex.Replace(firstPrompt.content, pattern, replacement);

        if (result.IndexOf("Please use this context: ", StringComparison.Ordinal) == -1)
        {
            result = string.Join("\n", new string[] { replacement, firstPrompt.content });
        }

        firstPrompt.content = result;

        return input;
    }
}