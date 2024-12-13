using AITextWriter.Infrastructure.Abstractions;
using AITextWriter.Services;
using AITextWriter.Services.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AITextWriter.Infrastructure.Tests;

public class UserPromptReaderTests
{
    [Fact]
    public async Task GetTagsAsync_MustReturnCorrectTags()
    {
        // Arrange
        var parametersProvider = Substitute.For<IFileSystemContextParameters>();
        parametersProvider.GetWorkingFolderPathAsync().Returns(x => Task.FromResult("testPath"));

        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.GetFilePathsAsync("testPath", "*.", false)
            .Returns(x =>
            {
                return Task.FromResult(new[]
                {
                    "testPath/c#", // this is tag
                    "testPath/cloud", // this is tag
                    "testPath/story", // this is tag

                    // these expected to be ignored
                    "testPath/AITextWriterListen", // this is app name, which can be on macos
                    "testPath/AITextWriterProcess", // this is app name, which can be on macos
                    "testPath/AITextWriterSummarize", // this is app name, which can be on macos
                    "testPath/apikey", // this is api key
                });
            });

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    x.AddScoped<IFileSystemContextParameters, IFileSystemContextParameters>(_ => parametersProvider);
                    return x;
                }
            )
            .GetService<IUserPromptReader>();

        var tags = await sut.GetTagsAsync("sample-file");

        // Assert
        await fileSystemProvider.Received(1).GetFilePathsAsync("testPath", "*.", false);
        tags.Should().HaveCount(3);
        tags.Should().Contain("c#");
        tags.Should().Contain("cloud");
        tags.Should().Contain("story");
    }

    [Fact]
    public async Task GetApiKey_MustReturnApiKeyContent()
    {
        var parametersProvider = Substitute.For<IFileSystemContextParameters>();
        parametersProvider.GetWorkingFolderPathAsync().Returns(x => Task.FromResult("testPath"));

        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.GetFilePathsAsync("testPath", "*.", false)
            .Returns(x =>
            {
                return Task.FromResult(new[]
                {
                    "testPath/c#", // this is tag
                    "testPath/cloud", // this is tag
                    "testPath/story", // this is tag

                    // these expected to be ignored
                    "testPath/AITextWriterListen", // this is app name, which can be on macos
                    "testPath/AITextWriterProcess", // this is app name, which can be on macos
                    "testPath/AITextWriterSummarize", // this is app name, which can be on macos
                    "testPath/apikey", // this is api key
                });
            });

        fileSystemProvider.GetFilePathsAsync("testPath", "apikey", false)
            .Returns(x => Task.FromResult(new[]
            {
                "testPath/apikey", // this is api key
            }));

        fileSystemProvider.ReadAllTextAsync("testPath/apikey").Returns(
            x => Task.FromResult("ABCDE"));

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    x.AddScoped<IFileSystemContextParameters, IFileSystemContextParameters>(_ => parametersProvider);
                    return x;
                }
            )
            .GetService<IUserPromptReader>();

        var apiKey = await sut.GetApiKeyAsync();

        // Assert
        await fileSystemProvider.Received(1).ReadAllTextAsync("testPath/apikey");
        apiKey.Should().Be("ABCDE");
    }


    [Theory]
    [InlineData("Hello! How are you?", 1)]
    [InlineData("Hello! How are you?\n\nPlease tell me something", 1)]
    [InlineData("Hello! How are you?\n\n\nPlease tell me something\nhere is some test data", 2)]
    [InlineData("Hello! How are you?\n\n\nPlease tell me something\n\n\nhere is some test data", 3)]
    [InlineData("Hello! How are you?\n\n\n\n\nPlease tell me something\n\n\nhere is some test data", 3)]
    [InlineData("AA\nBB\n\nCC", 1)]
    [InlineData("", 0)]
    [InlineData("FF", 1)]
    [InlineData("ABCDE\n ABCDE", 1)]
    public async Task GetPromptsAsync_MustSupportSetOfInputCases(string inputText, int expectedPromptCount)
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.ReadAllTextAsync("testpath/testfile").Returns(Task.FromResult(inputText));
        
        var parametersProvider = Substitute.For<IFileSystemContextParameters>();

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(x=>fileSystemProvider);
                    x.AddScoped<IFileSystemContextParameters, IFileSystemContextParameters>(x=>parametersProvider);
                    return x;
                }
            )
            .GetService<IUserPromptReader>();

        var prompts = await sut.GetPromptsAsync("testpath/testfile");

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
        
        var parametersProvider = Substitute.For<IFileSystemContextParameters>();

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(x=>fileSystemProvider);
                    x.AddScoped<IFileSystemContextParameters, IFileSystemContextParameters>(x=>parametersProvider);
                    return x;
                }
            )
            .GetService<IUserPromptReader>();

        var prompts = await sut.GetPromptsAsync("testpath/testfile");

        // Assert
        prompts.Should().NotBeEmpty();
        prompts.Should().HaveCount(1);
        
        prompts[0].role.Trim().Should().Be("user");
        prompts[0].content.Trim().Should().Be("This is an example \n of empty case");
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IUserPromptReader, UserPromptReader>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}