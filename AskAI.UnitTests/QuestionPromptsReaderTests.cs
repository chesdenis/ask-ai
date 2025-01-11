using AskAI.Infrastructure.Abstractions;
using AskAI.Services;
using AskAI.Services.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AskAI.UnitTests;

public class QuestionPromptsReaderTests
{
    [Theory]
    [InlineData("Hello! How are you?", 1)]
    [InlineData("Hello! How are you?\n---\nPlease tell me something", 2)]
    [InlineData("Hello! How are you?\n\n---\nPlease tell me something\nhere is some test data", 2)]
    [InlineData("Hello! How are you?\n---\n\nPlease tell me something\n---\n\nhere is some test data", 3)]
    [InlineData("Hello! How are you?\n\n---\n\n\nPlease tell me something\n---\n\nhere is some test data", 3)]
    [InlineData("AA\nBB\n\nCC", 1)]
    [InlineData("", 0)]
    [InlineData("FF", 1)]
    [InlineData("ABCDE\n ABCDE", 1)]
    public async Task GetPromptsAsync_MustSupportSetOfInputCases(string inputText, int expectedPromptCount)
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.ReadAllTextAsync("testpath/testfile").Returns(Task.FromResult(inputText));
        
        var parametersProvider = Substitute.For<IWorkSpaceContext>();

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(x=>fileSystemProvider);
                    x.AddScoped<IWorkSpaceContext, IWorkSpaceContext>(x=>parametersProvider);
                    return x;
                }
            )
            .GetService<IQuestionPromptsReader>();

        var prompts = await sut.ReadAsync("testpath/testfile");

        // Assert
        prompts.Should().HaveCount(expectedPromptCount);
    }
    
    [Theory]
    [InlineData("This is an example \n of empty case")]
    public async Task GetPrompts_MustSupportEmptyInputCase(string inputText)
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.ReadAllTextAsync("testpath/testfile").Returns(Task.FromResult(inputText));
        
        var parametersProvider = Substitute.For<IWorkSpaceContext>();

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(x=>fileSystemProvider);
                    x.AddScoped<IWorkSpaceContext, IWorkSpaceContext>(x=>parametersProvider);
                    return x;
                }
            )
            .GetService<IQuestionPromptsReader>();

        var prompts = await sut.ReadAsync("testpath/testfile");

        // Assert
        prompts.Should().NotBeEmpty();
        prompts.Should().HaveCount(1);
        
        prompts[0].role.Trim().Should().Be("user");
        prompts[0].content.Trim().Should().Be("This is an example \n of empty case");
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IQuestionPromptsReader, QuestionPromptsReader>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}