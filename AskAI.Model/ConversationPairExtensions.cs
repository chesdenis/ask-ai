namespace AskAI.Model;

public static class ConversationPairExtensions
{
    public static IEnumerable<Prompt> ToPrompts(this IEnumerable<ConversationPair> conversationPairs)
    {
        foreach (var conversationPair in conversationPairs)
        {
            yield return conversationPair.UserQuestion;
            if (conversationPair.AssistantAnswer is not null)
            {
                yield return conversationPair.AssistantAnswer;
            }
        }
    }
}