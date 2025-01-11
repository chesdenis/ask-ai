using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IQuestionsReader
{
    IAsyncEnumerable<Prompt> ReadAsync(string filePath);
}