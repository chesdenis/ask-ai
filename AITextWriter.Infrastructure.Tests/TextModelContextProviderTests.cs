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
        
        fileSystemProvider.GetFilePathsAsync( "testPath", "apikey", false)
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

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ITextModelContextProvider, TextModelContextProvider>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}