using AITextWriter.Model;

namespace AITextWriter.Services.Abstractions;

public interface IAssistantPromptReader
{
    Task<Prompt[]> GetPromptsAsync();
}