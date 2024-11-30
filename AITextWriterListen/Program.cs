using AITextWriter.Infrastructure;
using AITextWriter.Infrastructure.Abstractions;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AITextWriterListen;

internal class Program
{
    static readonly ServiceCollection serviceCollection = new();
    static void Main(string[] args)
    {
        ConfigureServices(serviceCollection);
        
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Get the logger
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            logger.LogInformation("Starting application");

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptionsAndReturnExitCode)
                .WithNotParsed(HandleParseError);
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

        services.AddScoped<IFileSystemNotifier, FileSystemNotifier>();
        services.AddScoped<IFileSystemProvider, FileSystemProvider>();
        services.AddScoped<IParametersProvider, ParametersProvider>();
    }

    static void RunOptionsAndReturnExitCode(Options opts)
    {
        if (opts.Help)
        {
            Log.Information("AI Text Writer - Listen for changes and write answer in the edited file");
            return;
        }

        // Your logic here
        Log.Information("Working Folder: {WorkingFolder}", opts.WorkingFolder);
        Log.Information("Model: {Model}", opts.Model);
        Log.Information("Verbose: {Verbose}", opts.Verbose);
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        foreach (var error in errs)
        {
            Log.Error("Error: {Error}", error);
        }
    }
}