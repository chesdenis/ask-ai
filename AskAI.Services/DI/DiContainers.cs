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
        IListenFolderOptions listenFolderOptions)
    {
        services.AddScoped<IFileEventsNotifier, FileEventsNotifier>();
        services.AddScoped<IFileSystemProvider, FileSystemProvider>();
        services.AddScoped<IWorkingContextParameters, WorkingContextParameters>();
        services.AddScoped<IAskPromptGenerator, AskPromptGenerator>();
        services.AddScoped<IUserPromptReader, UserPromptReader>();
        services.AddScoped<IAssistantPromptReader, AssistantPromptReader>();
        services.AddScoped<IPromptEnricher, PromptEnricher>();
        services.AddScoped<IListenFolderOptions>(_ => listenFolderOptions);

        return services;
    }
}