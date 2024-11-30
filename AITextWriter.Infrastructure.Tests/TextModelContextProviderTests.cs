using AITextWriter.Infrastructure.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AITextWriter.Infrastructure.Tests;

public class TextModelContextProviderTests
{
    [Fact]
    public async Task GetTagsAsync_MustReturnCorrectTags()
    {
        // Arrange
        var parametersProvider = Substitute.For<IParametersProvider>();
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
                    x.AddScoped<IParametersProvider, IParametersProvider>(_ => parametersProvider);
                    return x;
                }
            )
            .GetService<ITextModelContextProvider>();

        var tags = await sut.GetTagsAsync();

        // Assert
        await fileSystemProvider.Received(1).GetFilePathsAsync("testPath", "*.", false);
        tags.Should().HaveCount(3);
        tags.Should().Contain("c#");
        tags.Should().Contain("cloud");
        tags.Should().Contain("story");
    }

    [Fact]
    public async Task GetApiKey_ShouldReturnApiKeyContent()
    {
        var parametersProvider = Substitute.For<IParametersProvider>();
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
                    x.AddScoped<IParametersProvider, IParametersProvider>(_ => parametersProvider);
                    return x;
                }
            )
            .GetService<ITextModelContextProvider>();

        var apiKey = await sut.GetApiKey();

        // Assert
        await fileSystemProvider.Received(1).ReadAllTextAsync("testPath/apikey");
        apiKey.Should().Be("ABCDE");
    }


    [Theory]
    [InlineData("### user\nHello\n### assistant\nHi\n### user\nHow are you?\n### assistant\nI am fine\n")]
    [InlineData("### user   \nHello\n### assistant\nHi\n### user  \n\n\nHow are you?\n### assistant\nI am fine\n")]
    [InlineData("### user   \nHello\n### assistant\nHi\n### user  \n\n\nHow are you?\n### assistant  \n\n\n\nI am fine\n")]
    [InlineData("###user   \nHello\n###assistant\nHi\n###user  \n\n\nHow are you?\n###assistant  \n\n\n\nI am fine\n")]
    public async Task GetPrompts_MustSupportDifferentKindOfInputCases(string inputText)
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.ReadAllTextAsync("testpath/testfile").Returns(Task.FromResult(inputText));
        
        var parametersProvider = Substitute.For<IParametersProvider>();
        parametersProvider.GetWorkingFilePathAsync().Returns(Task.FromResult("testpath/testfile"));

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(x=>fileSystemProvider);
                    x.AddScoped<IParametersProvider, IParametersProvider>(x=>parametersProvider);
                    return x;
                }
            )
            .GetService<ITextModelContextProvider>();

        var prompts = await sut.GetPrompts();

        // Assert
        prompts.Should().NotBeEmpty();
        prompts.Should().HaveCount(4);
        prompts[0].role.Trim().Should().Be("user");
        prompts[0].content.Trim().Should().Be("Hello");
        
        prompts[1].role.Trim().Should().Be("assistant");
        prompts[1].content.Trim().Should().Be("Hi");
        
        prompts[2].role.Trim().Should().Be("user");
        prompts[2].content.Trim().Should().Be("How are you?");
        
        prompts[3].role.Trim().Should().Be("assistant");
        prompts[3].content.Trim().Should().Be("I am fine");
    }
    
    [Theory]
    [InlineData("This is an example \n of empty case")]
    public async Task GetPrompts_MustSupportEmptyInputCase(string inputText)
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        fileSystemProvider.ReadAllTextAsync("testpath/testfile").Returns(Task.FromResult(inputText));
        
        var parametersProvider = Substitute.For<IParametersProvider>();
        parametersProvider.GetWorkingFilePathAsync().Returns(Task.FromResult("testpath/testfile"));

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(x=>fileSystemProvider);
                    x.AddScoped<IParametersProvider, IParametersProvider>(x=>parametersProvider);
                    return x;
                }
            )
            .GetService<ITextModelContextProvider>();

        var prompts = await sut.GetPrompts();

        // Assert
        prompts.Should().NotBeEmpty();
        prompts.Should().HaveCount(1);
        
        prompts[0].role.Trim().Should().Be("user");
        prompts[0].content.Trim().Should().Be("This is an example \n of empty case");
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ITextModelContextProvider, TextModelContextProvider>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}