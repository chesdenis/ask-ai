using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Infrastructure.Options;

namespace AITextWriter.Infrastructure;

public class ParametersProvider(IListenContextOptions options)
    : IParametersProvider
{
    public Task<string> GetWorkingFolderPathAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetWorkingFilePathAsync()
    {
        throw new NotImplementedException();
    }
}