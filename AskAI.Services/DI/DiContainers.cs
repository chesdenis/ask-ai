using AskAI.Infrastructure;
using AskAI.Infrastructure.Abstractions;
using AskAI.OpenAI.Provider;
using AskAI.Services.Abstractions;
using AskAI.Services.Apps;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AskAI.Services.DI;

public static class DiContainers
{
    public static IServiceCollection RegisterLogging(this IServiceCollection services,
        LogLevel logLevel = LogLevel.Information)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        services.AddLogging(
                configure => configure.AddConsole())
            .Configure<LoggerFilterOptions>(
                options =>
                    options.MinLevel = logLevel);

        return services;
    }

    public static IServiceCollection AddAppsComponents(this IServiceCollection services)
    {
        services.AddScoped<IAssistantResponseProvider, OpenAiAssistantResponseProvider>()
            .AddHttpClient();
        
        services.AddScoped<IFileWatcher, FileWatcher>();
        services.AddScoped<IFileSystemProvider, FileSystemProvider>();
        services.AddScoped<IWorkSpaceContext, WorkSpaceContext>();
        services.AddScoped<IAskPromptGenerator, AskPromptGenerator>();
        services.AddScoped<IQuestionsReader, QuestionsReader>();
        services.AddScoped<IConversationReader, ConversationReader>();
        services.AddScoped<IPromptEnricher, PromptEnricher>();
        services.AddScoped<IAssistantAnswersWriter, AssistantAnswersWriter>();

        services.AddScoped<AskAiConsoleMode>();
        services.AddScoped<AskAiDocumentMode>();

        return services;
    }
}