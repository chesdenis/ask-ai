using AskAI.Infrastructure.Abstractions;
using AskAI.RunOptions;
using AskAI.Services.Abstractions;
using AskAI.Services.DI;
using Microsoft.Extensions.DependencyInjection;

namespace AskAI.UnitTests;

public class DiContainersTests
{
    [Fact]
    public void MustResolveAllFor_AITextWriterListen()
    {
        // Arrange
        var services = new ServiceCollection();
        var listenOptions = new WatchOptions
        {
            WorkingFolder = "testPath"
        };

        // Act
        services
            .RegisterLogging()
            .AddAppsComponents();
        
        var serviceProvider = services.BuildServiceProvider();
        
        var fileEventsNotifier = serviceProvider.GetRequiredService<IFileWatcher>();
        var userPromptReader = serviceProvider.GetRequiredService<IQuestionsReader>();
        var assistancePromptReader = serviceProvider.GetRequiredService<IConversationReader>();
        var askPromptGenerator = serviceProvider.GetRequiredService<IAskPromptGenerator>();
        var promptEnricher = serviceProvider.GetRequiredService<IPromptEnricher>();
        
        // Assert
    }
}