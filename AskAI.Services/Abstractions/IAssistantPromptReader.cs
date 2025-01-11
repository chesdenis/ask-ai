using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IAssistantPromptReader
{
    Task<Prompt[]> GetPromptsAsync(string filePath);
}