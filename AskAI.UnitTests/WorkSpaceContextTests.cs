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
        fileSystemProvider.GetFilePathsAsync("testDir", "*.", false)
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

        var tags = await sut.GetTagsAsync("testDir/testPath");

        // Assert
        await fileSystemProvider.Received(1).GetFilePathsAsync("testDir", "*.", false);
        tags.Should().HaveCount(3);
        tags.Should().Contain("c#");
        tags.Should().Contain("cloud");
        tags.Should().Contain("story");
    }
    
    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IWatchOptions, IWatchOptions>(_ => new WatchOptions
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