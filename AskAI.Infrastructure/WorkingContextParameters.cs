using AskAI.Infrastructure.Abstractions;
using AskAI.Infrastructure.Options;

namespace AskAI.Infrastructure;

public class WorkingContextParameters(IListenFolderOptions folderOptions)
    : IWorkingContextParameters
{
    public Task<string> GetWorkingFolderPathAsync() => Task.FromResult(folderOptions.WorkingFolder);
}