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

            // Parser.Default.ParseArguments<ListenOptions>(args).WithParsed(RunOptionsAndReturnExitCode)
            //     .WithNotParsed(HandleParseError);

            // if (options == null)
            // {
            //     throw new Exception("Startup parameters are not set");
            // }

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

//     static void RunOptionsAndReturnExitCode(ListenOptions opts)
//     {
//         if (opts.Help)
//         {
//             Log.Information("AI Text Writer - Listen for changes and write answer in the edited file");
//             return;
//         }
//
//         Log.Information("Working Folder: {WorkingFolder}", opts.WorkingFolder);
//         Log.Information("Verbose: {Verbose}", opts.Verbose);
//
//         options.WorkingFolder = opts.WorkingFolder;
//         options.Verbose = opts.Verbose;
//         options.Help = opts.Help;
//     }
//
//     static void HandleParseError(IEnumerable<Error> errs)
//     {
//         foreach (var error in errs)
//         {
//             Log.Error("Error: {Error}", error);
//         }
//     }
//
//     static void AppEntryPoint(CancellationToken ct)
//     {
//         var serviceProvider = serviceCollection.BuildServiceProvider();
//         var fileEventsNotifier = serviceProvider.GetRequiredService<IFileEventsNotifier>();
//         var userPromptReader = serviceProvider.GetRequiredService<IUserPromptReader>();
//         var assistancePromptReader = serviceProvider.GetRequiredService<IAssistantPromptReader>();
//         var askPromptGenerator = serviceProvider.GetRequiredService<IAskPromptGenerator>();
//         var promptEnricher = serviceProvider.GetRequiredService<IPromptEnricher>();
//         var assistantResponseProvider = serviceProvider.GetRequiredService<IAssistantResponseProvider>();
//
//         fileEventsNotifier.Start(options.WorkingFolder);
//         fileEventsNotifier.FileChanged += (sender, args) =>
//         {
//             try
//             {
//                 switch (args.ChangeType)
//                 {
//                     case WatcherChangeTypes.Deleted:
//                         Log.Logger.Information("Skipping event for deleted file: {FullPath}", args.FullPath);
//                         return;
//                     case WatcherChangeTypes.Renamed:
//                         Log.Logger.Information("Skipping event for renamed file: {FullPath}", args.FullPath);
//                         return;
//                 }
//
//                 var attr = File.GetAttributes(args.FullPath);
//                 if (attr.HasFlag(FileAttributes.Hidden))
//                 {
//                     Log.Logger.Information("Skipping event for system file: {FullPath}", args.FullPath);
//                     return;
//                 }
//
//
//                 var systemFiles = new HashSet<string?>()
//                 {
//                     "apikey",
//                     "model",
//                     "endpoint",
//                     "timeout"
//                 };
//                 if (args.FullPath.ToLowerInvariant().EndsWith(ContextExtensions.GetAnswerSuffix()))
//                 {
//                     Log.Logger.Information("Skipping event for system file: {FullPath}", args.FullPath);
//                     return;
//                 }
//
//                 if (systemFiles.Contains(args.Name?.ToLowerInvariant()))
//                 {
//                     Log.Logger.Information("Skipping event for system file: {FullPath}", args.FullPath);
//                     return;
//                 }
//
//                 var workingFilePath = args.FullPath;
//                 Log.Information("Incoming file: {WorkingFilePath}", workingFilePath);
//
//                 var userPrompts = userPromptReader.GetPromptsAsync(workingFilePath)
//                     .ConfigureAwait(false)
//                     .GetAwaiter().GetResult();
//                 Log.Information("Got user prompts: {UserPromptsLength}", userPrompts.Length);
//
//                 var assistantPrompts = assistancePromptReader.GetPromptsAsync(workingFilePath)
//                     .ConfigureAwait(false)
//                     .GetAwaiter().GetResult();
//                 Log.Information("Got assistant prompts: {AssistantPromptsLength}", assistantPrompts.Length);
//
//                 var enrichedPrompts = promptEnricher.EnrichAsync(userPrompts, workingFilePath)
//                     .ConfigureAwait(false)
//                     .GetAwaiter().GetResult();
//                 Log.Information("Prepared enriched prompts: {EnrichedPromptsLength}", enrichedPrompts.Length);
//
//                 var askPrompts = askPromptGenerator.GenerateAskPromptAsync(enrichedPrompts, assistantPrompts)
//                     .ConfigureAwait(false)
//                     .GetAwaiter().GetResult();
//                 Log.Information("Generated ask prompts: {AskPromptsLength}", askPrompts.Length);
//
//                 if (askPrompts.Length == 0)
//                 {
//                     Log.Information("No prompts to ask.");
//                     return;
//                 }
//
//                 var answer = assistantResponseProvider.GetAssistantAnswer(askPrompts,
//                         new ModelDetails
//                             { Model = ContextExtensions.ResolveRequiredKey(options.WorkingFolder, "model") },
//                         new ApiRequestSettings
//                         {
//                             ApiKey = ContextExtensions.ResolveRequiredKey(options.WorkingFolder, "apikey"),
//                             Model = ContextExtensions.ResolveRequiredKey(options.WorkingFolder, "model"),
//                             Endpoint = ContextExtensions.ResolveRequiredKey(options.WorkingFolder, "endpoint"),
//                             TimeoutMinutes =
//                                 Convert.ToDouble(
//                                     ContextExtensions.ResolveRequiredKey(options.WorkingFolder, "timeout"))
//                         })
//                     .ConfigureAwait(false)
//                     .GetAwaiter().GetResult();
//                 Log.Information("Got answer: {AnswerLength}", answer.Length);
//
//                 var answerFilePathAsync = workingFilePath.GetAnswerFilePathAsync()
//                     .ConfigureAwait(false)
//                     .GetAwaiter().GetResult();
//
//                 var outputPrompts = askPrompts.Append(new Prompt
//                 {
//                     role = "assistant",
//                     content = answer
//                 }).ToArray();
//                 
//                 var sb = new StringBuilder();
//
//                 foreach (var prompt in outputPrompts)
//                 {
//                     sb.AppendLine($"### {prompt.role}");
//                     sb.AppendLine(prompt.content);
//                     sb.AppendLine();
//                 }
//
//                 File.WriteAllText(answerFilePathAsync, sb.ToString());
//             }
//             catch (Exception e)
//             {
//                 Log.Error("Error: {Error}", e);
//             }
//         };
//
//         try
//         {
//             while (!ct.IsCancellationRequested)
//             {
//                 Thread.Sleep(3000);
//             }
//         }
//         finally
//         {
//             fileEventsNotifier.Stop();
//         }
//     }
// }