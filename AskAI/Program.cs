using AskAI.Infrastructure.Abstractions;
using AskAI.OpenAI.Provider;
using AskAI.Services.Apps;
using AskAI.Services.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AskAI;

internal class Program
{
    private static readonly ServiceCollection ServiceCollection = new();

    static async Task Main(string[] args)
    {
        ConfigureServices(ServiceCollection);

        var serviceProvider = ServiceCollection.BuildServiceProvider();

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

            if (args.Length == 0)
            {
                // run regular simple question and answer mode
                await serviceProvider.GetRequiredService<AskAiConsoleMode>().RunAsync(ct);
            }

            if (args.Length == 1)
            {
                // run app with question document
                await serviceProvider.GetRequiredService<AskAiDocumentMode>().RunAsync(args[0], ct);
            }
            
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .RegisterLogging()
            .AddAppsComponents();
    }
}