namespace AskAI.Services.Extensions;

public static class ContextExtensions
{
    public static string GetAnswerSuffix() => ".answer.md";

    public static Task<string> GetAnswerFilePathAsync(this string filePath) => Task.FromResult($"{filePath}{GetAnswerSuffix()}");

    public static T ResolveRequiredKey<T>(string workingFolderPath, string keyName)
    {
        var keyPath = Path.Combine(workingFolderPath, keyName);
        if (!File.Exists(keyPath))
        {
            throw new ArgumentNullException(nameof(keyPath),
                $"Key file: {keyName} not found in folder {workingFolderPath}");
        }

        var valueAsText = File.ReadAllText(keyPath);
        
        return (T)Convert.ChangeType(valueAsText, typeof(T));
    }
}