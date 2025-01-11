using AskAI.Infrastructure;
using AskAI.Infrastructure.Abstractions;
using AskAI.Infrastructure.Options;
using AskAI.RunOptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace AskAI.UnitTests;

public class WorkSpaceContextTests
{
    [Fact]
    public async Task GetTagsAsync_MustReturnCorrectTags()
    {
        // Arrange
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
                    "testPath/AskAI", // this is app name, which can be on macos
                    "testPath/apikey", // this is api key
                });
            });

        // Act
        var sut = BuildServices(x =>
                {
                    x.AddScoped<IFileSystemProvider, IFileSystemProvider>(_ => fileSystemProvider);
                    x.AddScoped<IWorkSpaceContext, WorkSpaceContext>();
                    return x;
                }
            )
            .GetService<IWorkSpaceContext>();

        var tags = await sut.GetTagsAsync("testPath");

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
                    "testPath/AskAI", // this is app name, which can be on macos
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
                    x.AddScoped<IWorkSpaceContext, WorkSpaceContext>();
                    return x;
                }
            )
            .GetService<IWorkSpaceContext>();

        var apiKey = await sut.GetApiKeyAsync();

        // Assert
        await fileSystemProvider.Received(1).ReadAllTextAsync("testPath/apikey");
        apiKey.Should().Be("ABCDE");
    }
    
    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IWatchOptions, IWatchOptions>(_ => new WatchFolderOptions
        {
            WorkingFolder = "testPath",
            Verbose = false,
            Help = false
        });
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }

}