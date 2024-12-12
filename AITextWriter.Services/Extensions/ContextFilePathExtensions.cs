namespace AITextWriter.Services.Extensions;

public static class ContextFilePathExtensions
{
    public static Task<string> GetAnswerFilePathAsync(this string filePath)
    {
        return Task.FromResult($"{filePath}.answer.md");
    }
}