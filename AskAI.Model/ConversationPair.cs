namespace AskAI.Model;

public record ConversationPair
{
    public required Prompt UserQuestion { get; set; }

    public Prompt? AssistantAnswer { get; set; }

    public string UserQuestionHash => UserQuestion.ToStringHash();
    public string AssistantAnswerHash => AssistantAnswer?.ToStringHash() ?? string.Empty;
}