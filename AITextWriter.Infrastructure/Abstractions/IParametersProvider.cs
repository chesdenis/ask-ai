namespace AITextWriter.Infrastructure.Abstractions;

public interface IParametersProvider
{
    Task<string> GetWorkingFolderPathAsync();
    Task<string> GetWorkingFilePathAsync();
}