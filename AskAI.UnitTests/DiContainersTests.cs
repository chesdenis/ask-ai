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
        var listenOptions = new ListenFolderFolderOptions
        {
            WorkingFolder = "testPath"
        };

        // Act
        services
            .RegisterLogging()
            .RegisterWriterListener(listenOptions);
        
        var serviceProvider = services.BuildServiceProvider();
        
        var fileEventsNotifier = serviceProvider.GetRequiredService<IFileEventsNotifier>();
        var userPromptReader = serviceProvider.GetRequiredService<IUserPromptReader>();
        var assistancePromptReader = serviceProvider.GetRequiredService<IAssistantPromptReader>();
        var askPromptGenerator = serviceProvider.GetRequiredService<IAskPromptGenerator>();
        var promptEnricher = serviceProvider.GetRequiredService<IPromptEnricher>();
        
        // Assert
    }
}