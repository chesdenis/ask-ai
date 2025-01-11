namespace AskAI.Infrastructure.Abstractions;

public interface IWorkSpaceContext
{
    Task<string> GetWorkingFolderPathAsync();
    Task<string[]> GetTagsAsync(string filePath);
    Task<string> GetApiKeyAsync();
}