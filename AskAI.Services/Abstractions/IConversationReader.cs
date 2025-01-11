using AskAI.Model;

namespace AskAI.Services.Abstractions;

public interface IConversationReader
{
    IAsyncEnumerable<ConversationPair> EnumerateConversationPairsAsync(string questionFilePath);
}