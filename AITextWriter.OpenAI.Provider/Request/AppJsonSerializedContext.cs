using System.Text.Json.Serialization;
using AITextWriter.Model;

namespace AITextWriter.OpenAI.Provider.Request;

[JsonSerializable(typeof(AiRequest))]
[JsonSerializable(typeof(ChatCompletionResponse))]
[JsonSerializable(typeof(Choice))]
[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Prompt))]
[JsonSerializable(typeof(Usage))]
internal partial class AppJsonSerializedContext : JsonSerializerContext
{
}