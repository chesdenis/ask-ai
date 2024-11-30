using AITextWriter.Model;

namespace AITextWriter.Services.Abstractions;

public interface IPromptEnricher
{
    Task<Prompt[]> EnrichAsync(Prompt[] input);
}