using AITextWriter.Model;

namespace AITextWriter.Services.Abstractions;

public interface IUserPromptReader
{
    Task<string[]> GetTagsAsync();
    Task<string> GetApiKeyAsync();
    Task<Prompt[]> GetPromptsAsync();
}