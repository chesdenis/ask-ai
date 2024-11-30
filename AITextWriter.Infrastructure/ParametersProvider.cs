using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Infrastructure.Options;

namespace AITextWriter.Infrastructure;

public class ParametersProvider : IParametersProvider
{
    private readonly IListenContextOptions _options;

    public ParametersProvider(IListenContextOptions options)
    {
        _options = options;
    }

    public ParametersProvider()
    {
        
    }
    public Task<string> GetWorkingFolderPathAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string> GetWorkingFilePathAsync()
    {
        throw new NotImplementedException();
    }
}