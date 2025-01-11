using System.Net.Http.Json;
using System.Text.Json;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.OpenAI.Provider.Request;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AskAI.OpenAI.Provider;

public class OpenAiAssistantResponseProvider(
    IHttpClientFactory httpClientFactory,
    ILogger<OpenAiAssistantResponseProvider> logger) : IAssistantResponseProvider
{
    public async Task<string> GetAssistantAnswer(
        Prompt[] prompts, 
        ApiRequestSettings requestSettings)
    {
        logger.LogDebug("Starting GetAssistantAnswer with model and endpoint: {Endpoint}", requestSettings.Endpoint);
        
        using var httpClient = httpClientFactory.CreateClient();
        ConfigureHttpClient(httpClient,requestSettings);

        var opts = new JsonSerializerOptions
        {
            TypeInfoResolver = AppJsonSerializedContext.Default
        };

        logger.LogDebug("Sending request to API.");

        string resultAsString;
        
        try
        {
            var response = await httpClient.PostAsJsonAsync(
                requestSettings.Endpoint, new AiRequest
                {
                    model = requestSettings.Model,
                    messages = prompts
                }, opts);

            response.EnsureSuccessStatusCode();
            
           
            resultAsString = await response.Content.ReadAsStringAsync();

            logger.LogDebug("Received response with size: {resultLength}", resultAsString.Length);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while sending and handling request to API");
            throw;
        }
        
        var result = JsonConvert.DeserializeObject<ChatCompletionResponse>(resultAsString);

        var message = result?.Choices.FirstOrDefault()?.Message;
        
        return message?.Content!;
    }
    
    private void ConfigureHttpClient(HttpClient httpClient, ApiRequestSettings requestSettings)
    {
        httpClient.DefaultRequestHeaders.Clear();
        // fix timeout property set issue!
        httpClient.Timeout = TimeSpan.FromMinutes(requestSettings.TimeoutMinutes);
        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {requestSettings.ApiKey}");
        }
    }
}