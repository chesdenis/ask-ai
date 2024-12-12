using System.Net.Http.Json;
using System.Text.Json;
using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Model;
using AITextWriter.OpenAI.Provider.Request;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AITextWriter.OpenAI.Provider;

public class OpenAiAssistantResponseProvider(
    HttpClient httpClient,
    ILogger<OpenAiAssistantResponseProvider> logger) : IAssistantResponseProvider
{
    public async Task<string> GetAssistantAnswer(
        Prompt[] prompts, ModelDetails modelDetails,
        ApiRequestSettings requestSettings)
    {
        logger.LogDebug("Starting GetAssistantAnswer with model and endpoint: {Endpoint}", requestSettings.Endpoint);

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.Timeout = TimeSpan.FromMinutes(requestSettings.TimeoutMinutes);
        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {requestSettings.ApiKey}");
        }

        var opts = new JsonSerializerOptions
        {
            TypeInfoResolver = AppJsonSerializedContext.Default
        };

        logger.LogDebug("Sending request to API.");
        var response = await httpClient.PostAsJsonAsync(
            requestSettings.Endpoint, new AiRequest
            {
                model = modelDetails.Model,
                messages = prompts
            }, opts);

        response.EnsureSuccessStatusCode();

        var resultAsString = await response.Content.ReadAsStringAsync();

        logger.LogDebug("Received response with size: {resultLength}", resultAsString.Length);

        var result = JsonConvert.DeserializeObject<ChatCompletionResponse>(resultAsString);

        var message = result?.Choices.FirstOrDefault()?.Message;
        
        return message?.Content!;
    }
}