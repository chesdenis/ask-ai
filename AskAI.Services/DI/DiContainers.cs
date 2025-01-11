using AskAI.Infrastructure;
using AskAI.Infrastructure.Abstractions;
using AskAI.Infrastructure.Options;
using AskAI.Services.Abstractions;
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

    public static IServiceCollection RegisterWriterListener(this IServiceCollection services,
        IWatchOptions watchOptions)
    {
        services.AddScoped<IFileWatcher, FileWatcher>();
        services.AddScoped<IFileSystemProvider, FileSystemProvider>();
        services.AddScoped<IWorkSpaceContext, WorkSpaceContext>();
        services.AddScoped<IAskPromptGenerator, AskPromptGenerator>();
        services.AddScoped<IQuestionPromptsReader, QuestionPromptsReader>();
        services.AddScoped<IConversationReader, ConversationReader>();
        services.AddScoped<IPromptEnricher, PromptEnricher>();
        services.AddScoped<IWatchOptions>(_ => watchOptions);

        return services;
    }
}