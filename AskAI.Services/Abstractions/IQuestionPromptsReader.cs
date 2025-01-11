using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IQuestionPromptsReader
{
    Task<Prompt[]> ReadAsync(string filePath);
}