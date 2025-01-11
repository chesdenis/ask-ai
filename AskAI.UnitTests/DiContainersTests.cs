using AskAI.Infrastructure.Abstractions;
using AskAI.RunOptions;
using AskAI.Services.Abstractions;
using AskAI.Services.Apps;
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
        
        serviceProvider.GetRequiredService<IAskPromptGenerator>();
        serviceProvider.GetRequiredService<IAssistantAnswersWriter>();
        serviceProvider.GetRequiredService<IConversationReader>();
        serviceProvider.GetRequiredService<IPromptEnricher>();
        serviceProvider.GetRequiredService<IQuestionsReader>();
        
        serviceProvider.GetRequiredService<IFileWatcher>();
        serviceProvider.GetRequiredService<IAssistantResponseProvider>();
        serviceProvider.GetRequiredService<IFileSystemProvider>();
        serviceProvider.GetRequiredService<IWorkSpaceContext>();
      
        serviceProvider.GetRequiredService<IConversationReader>();
        serviceProvider.GetRequiredService<IQuestionsReader>();
        
        serviceProvider.GetRequiredService<AskAiConsoleMode>();
        serviceProvider.GetRequiredService<AskAiDocumentMode>();

        // Assert
    }
}