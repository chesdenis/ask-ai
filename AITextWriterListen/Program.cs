using AITextWriter.Infrastructure;
using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Services.Abstractions;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AITextWriterListen;

internal class Program
{
    private static AiTextWriterListenOptions options;
    static readonly ServiceCollection serviceCollection = new();
    static void Main(string[] args)
    {
        ConfigureServices(serviceCollection);
        
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Get the logger
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        var cts = new CancellationTokenSource();
        var ct = cts.Token;

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };
        
        try
        {
            logger.LogInformation("Starting application");

            Parser.Default.ParseArguments<AiTextWriterListenOptions>(args).WithParsed(RunOptionsAndReturnExitCode).WithNotParsed(HandleParseError);

            if (options == null)
            {
                throw new Exception("Startup parameters are not set");
            }
            
            AppEntryPoint(ct);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
    
    private static void ConfigureServices(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        
        services.AddLogging(
                configure => configure.AddConsole())
            .Configure<LoggerFilterOptions>(
                options => 
                    options.MinLevel = LogLevel.Information);

        services.AddScoped<IFileEventsNotifier, FileEventsNotifier>();
        services.AddScoped<IFileSystemProvider, FileSystemProvider>();
        services.AddScoped<IFileSystemContextParameters, FileSystemContextParameters>();
    }

    static void RunOptionsAndReturnExitCode(AiTextWriterListenOptions opts)
    {
        if (opts.Help)
        {
            Log.Information("AI Text Writer - Listen for changes and write answer in the edited file");
            return;
        }
        Log.Information("Working Folder: {WorkingFolder}", opts.WorkingFolder);
        Log.Information("Model: {Model}", opts.Model);
        Log.Information("Verbose: {Verbose}", opts.Verbose);
        options = opts;
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        foreach (var error in errs)
        {
            Log.Error("Error: {Error}", error);
        }
    }
    
    static void AppEntryPoint(CancellationToken ct)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var fileEventsNotifier = serviceProvider.GetRequiredService<IFileEventsNotifier>();
        var userPromptReader = serviceProvider.GetRequiredService<IUserPromptReader>();
        var assistancePromptReader = serviceProvider.GetRequiredService<IAssistantPromptReader>();
        var promptEnricher = serviceProvider.GetRequiredService<IPromptEnricher>();
        
        fileEventsNotifier.Start(options.WorkingFolder);
        fileEventsNotifier.FileChanged += (sender, args) =>
        {
            var workingFilePath = args.FullPath;
            var userPrompts = userPromptReader.GetPromptsAsync(workingFilePath)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();

            var assistantPrompts = assistancePromptReader.GetPromptsAsync(workingFilePath)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();

            var enrichedPrompts = promptEnricher.EnrichAsync(userPrompts, workingFilePath)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();
        };
        try
        {
            while (!ct.IsCancellationRequested)
            {
                Thread.Sleep(30000);
            }
        }
        finally
        {
            fileEventsNotifier.Stop();
        }
    }
}