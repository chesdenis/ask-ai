using AITextWriter.Model;

namespace AITextWriter.Services;

public interface IAskPromptGenerator
{
    Task<Prompt[]> GenerateAskPromptAsync(Prompt[] userPrompts, Prompt[] assistantPrompts);
}