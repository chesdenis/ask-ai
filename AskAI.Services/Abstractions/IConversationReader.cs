using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IConversationReader
{
    IAsyncEnumerable<ConversationPair> EnumerateAsync(string filePath);
}