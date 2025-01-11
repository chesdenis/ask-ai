namespace AskAI.Services.Extensions;

public static class TaskExtensions
{
    public static async Task<T> TryWithFallbackValueAs<T>(this Task<T> task, T defaultValue)
    {
        try
        {
            return await task.ConfigureAwait(false);
        }
        catch
        {
            return defaultValue;
        }
    }
}