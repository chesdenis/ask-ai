using AITextWriter.Model;
using AITextWriter.Services.Extensions;

namespace AITextWriter.Services;

public class AskPromptGenerator : IAskPromptGenerator
{
    public Task<Prompt[]> GenerateAskPromptAsync(Prompt[] userPrompts, Prompt[] assistantPrompts)
    {
        var userPromptsSet = userPrompts.ToDictionary(
            x => x.ToStringHash(),
            y => y);

        var assistantPromptsSet = assistantPrompts.ToDictionary(
            x => x.ToStringHash(),
            y => y);

        var result = new List<Prompt>();

        foreach (var promptKey in assistantPromptsSet.Keys)
        {
            if (userPromptsSet.ContainsKey(promptKey))
            {
                userPromptsSet.Remove(promptKey); // Exclude user prompts where we have answers already
            }
        }

        result.AddRange(assistantPrompts.Where(w => !string.IsNullOrWhiteSpace(w.content)));
        result.AddRange(userPromptsSet.Values.Where(w => !string.IsNullOrWhiteSpace(w.content)));

        return Task.FromResult(result.ToArray());
    }
}