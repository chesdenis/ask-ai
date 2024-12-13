using AITextWriter.Model;

namespace AITextWriter.Services.Abstractions;

public interface IUserPromptReader
{
    Task<string[]> GetTagsAsync(string filePath);
    Task<string> GetApiKeyAsync();
    Task<Prompt[]> GetPromptsAsync(string filePath);
}