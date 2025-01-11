using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using AskAI.Services.Abstractions;
using System.Text;

namespace AskAI.UnitTests;

public class AssistantAnswersWriterTests
{
    [Fact]
    public async Task WriteConversationAsync_EmptyPrompts_WritesEmptyFile()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    return x;
                }
            )
            .GetService<IAssistantAnswersWriter>();

        var prompts = Array.Empty<Prompt>();
        var workingDocument = "test.md";

        // Act
        await sut.WriteConversationAsync(prompts, workingDocument);

        // Assert
        await fileSystemProvider.Received(1).WriteAllTextAsync(Arg.Any<string>(), string.Empty);
    }

    [Fact]
    public async Task WriteConversationAsync_SinglePrompt_WritesCorrectContent()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    return x;
                }
            )
            .GetService<IAssistantAnswersWriter>();

        var prompts = new[]
        {
            new Prompt { role = "user", content = "User question" }
        };
        var workingDocument = "test.md";

        // Act
        await sut.WriteConversationAsync(prompts, workingDocument);

        // Assert
        var expectedContent = new StringBuilder()
            .AppendLine("### user")
            .AppendLine("User question")
            .AppendLine()
            .ToString();

        await fileSystemProvider.Received(1).WriteAllTextAsync(Arg.Any<string>(), expectedContent);
    }

    [Fact]
    public async Task WriteConversationAsync_MultiplePrompts_WritesCorrectContent()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    return x;
                }
            )
            .GetService<IAssistantAnswersWriter>();

        var prompts = new[]
        {
            new Prompt { role = "user", content = "User question 1" },
            new Prompt { role = "assistant", content = "Assistant answer 1" },
            new Prompt { role = "user", content = "User question 2" },
            new Prompt { role = "assistant", content = "Assistant answer 2" }
        };
        var workingDocument = "test.md";

        // Act
        await sut.WriteConversationAsync(prompts, workingDocument);

        // Assert
        var expectedContent = new StringBuilder()
            .AppendLine("### user")
            .AppendLine("User question 1")
            .AppendLine()
            .AppendLine("### assistant")
            .AppendLine("Assistant answer 1")
            .AppendLine()
            .AppendLine("### user")
            .AppendLine("User question 2")
            .AppendLine()
            .AppendLine("### assistant")
            .AppendLine("Assistant answer 2")
            .AppendLine()
            .ToString();

        await fileSystemProvider.Received(1).WriteAllTextAsync(Arg.Any<string>(), expectedContent);
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IAssistantAnswersWriter, AssistantAnswersWriter>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}