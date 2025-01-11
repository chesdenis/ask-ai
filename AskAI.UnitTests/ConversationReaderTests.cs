using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using AskAI.Services.Abstractions;

namespace AskAI.UnitTests;


public class ConversationReaderTests
{
    [Fact]
    public async Task EnumerateAsync_FileDoesNotExist_YieldsNothing()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.FileExist(Arg.Any<string>()).Returns(false);

        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    return x;
                }
            )
            .GetService<IConversationReader>();

        // Act
        var result = sut.EnumerateConversationPairsAsync("test.txt");
        var items = new List<ConversationPair>();
        await foreach (var item in result)
        {
            items.Add(item);
        }

        // Assert
        items.Should().BeEmpty();
    }

    [Fact]
    public async Task EnumerateAsync_FileContentWithoutRole_YieldsSingleConversationPair()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.FileExist(Arg.Any<string>()).Returns(true);
        fileSystemProvider.ReadAllTextAsync(Arg.Any<string>()).Returns(Task.FromResult("This is a user question without role."));

        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    return x;
                }
            )
            .GetService<IConversationReader>();

        // Act
        var result = sut.EnumerateConversationPairsAsync("test.txt");
        var items = new List<ConversationPair>();
        await foreach (var item in result)
        {
            items.Add(item);
        }

        // Assert
        items.Should().HaveCount(1);
        items[0].UserQuestion.role.Should().Be( ReservedKeywords.User);
        items[0].UserQuestion.content.Should().Be("This is a user question without role.");
    }

    [Fact]
    public async Task EnumerateAsync_FileContentWithMultiplePairs_YieldsCorrectConversationPairs()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.FileExist(Arg.Any<string>()).Returns(true);
        fileSystemProvider.ReadAllTextAsync(Arg.Any<string>()).Returns(Task.FromResult("### user\nUser question 1\n### assistant\nAssistant answer 1\n### user\nUser question 2\n### assistant\nAssistant answer 2"));

        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    return x;
                }
            )
            .GetService<IConversationReader>();

        // Act
        var result = sut.EnumerateConversationPairsAsync("test.txt");
        var items = new List<ConversationPair>();
        await foreach (var item in result)
        {
            items.Add(item);
        }

        // Assert
        items.Should().HaveCount(2);
        items[0].UserQuestion.role.Should().Be( ReservedKeywords.User);
        items[0].UserQuestion.content.Should().Be("User question 1");
        items[0].AssistantAnswer.role.Should().Be("assistant");
        items[0].AssistantAnswer.content.Should().Be("Assistant answer 1");
        items[1].UserQuestion.role.Should().Be( ReservedKeywords.User);
        items[1].UserQuestion.content.Should().Be("User question 2");
        items[1].AssistantAnswer.role.Should().Be("assistant");
        items[1].AssistantAnswer.content.Should().Be("Assistant answer 2");
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IConversationReader, ConversationReader>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}