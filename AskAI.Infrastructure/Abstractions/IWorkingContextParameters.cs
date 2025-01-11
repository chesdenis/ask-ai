namespace AskAI.Infrastructure.Abstractions;

public interface IWorkingContextParameters
{
    Task<string> GetWorkingFolderPathAsync();
}