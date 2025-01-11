using System.Text.Json.Serialization;
using AskAI.Model;

namespace AskAI.OpenAI.Provider.Request;

[JsonSerializable(typeof(AiRequest))]
[JsonSerializable(typeof(ChatCompletionResponse))]
[JsonSerializable(typeof(Choice))]
[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Prompt))]
[JsonSerializable(typeof(Usage))]
internal partial class AppJsonSerializedContext : JsonSerializerContext
{
}