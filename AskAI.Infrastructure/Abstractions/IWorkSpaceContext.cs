namespace AskAI.Infrastructure.Abstractions;

public interface IWorkSpaceContext
{
    Task<string[]> GetTagsAsync(string filePath);
}