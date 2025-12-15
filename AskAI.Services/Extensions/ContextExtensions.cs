namespace AskAI.Services.Extensions;

public static class ContextExtensions
{
    private const string ConversationSuffix = ".conversation.md";
    private const string AnswerSuffix = ".answer.md";

    public static Task<string> GetConversationFilePathAsync(this string filePath) =>
        Task.FromResult($"{filePath}{ConversationSuffix}");

    public static Task<string> GetAnswerFilePathAsync(this string filePath) =>
        Task.FromResult($"{filePath}{AnswerSuffix}");

    public static T ResolveRequiredKey<T>(string workingFolderPath, string keyName)
    {
        var envVar = Environment.GetEnvironmentVariable(keyName);
        
        if(envVar != null)
            return (T)Convert.ChangeType(envVar, typeof(T));
        
        var keyPath = Path.Combine(workingFolderPath, keyName);
        
        if (!File.Exists(keyPath))
        {
            // in case if key is not found in folder, we try to find it where main process sits
            var appBaseDir = AppContext.BaseDirectory;
            keyPath = Path.Combine(appBaseDir, keyName);
            if (!File.Exists(keyPath))
                throw new ArgumentNullException(nameof(keyPath),
                    $"Key file: {keyName} not found in folder {workingFolderPath}");
        }

        var valueAsText = File.ReadAllText(keyPath);

        return (T)Convert.ChangeType(valueAsText, typeof(T));
    }
}