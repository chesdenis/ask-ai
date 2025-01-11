using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IPromptEnricher
{
    Task<Prompt[]> EnrichAsync(Prompt[] input, string filePath);
}