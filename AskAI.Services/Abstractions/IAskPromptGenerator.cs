using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IAskPromptGenerator
{
    Task<Prompt[]> GenerateAskPromptAsync(Prompt[] userPrompts, Prompt[] assistantPrompts);
}