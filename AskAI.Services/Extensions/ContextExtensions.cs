namespace AskAI.Services.Extensions;

public static class ContextExtensions
{
    public static string GetAnswerSuffix() => ".answer.md";

    public static Task<string> GetAnswerFilePathAsync(this string filePath) => Task.FromResult($"{filePath}{GetAnswerSuffix()}");

    public static string ResolveRequiredKey(string workingFolderPath, string keyName)
    {
        var keyPath = Path.Combine(workingFolderPath, keyName);
        if (!File.Exists(keyPath))
        {
            throw new ArgumentNullException(nameof(keyPath),
                $"Key file not found in folder {workingFolderPath}");
        }

        return File.ReadAllText(keyPath);
    }
}