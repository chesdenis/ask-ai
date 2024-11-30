namespace AITextWriter.Infrastructure.Abstractions;

public interface ITextModelContextProvider
{
    Task<string[]> GetTagsAsync();
    Task<string> GetApiKey();
}