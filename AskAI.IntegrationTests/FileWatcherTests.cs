using AskAI.Infrastructure;
using AskAI.Infrastructure.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AITextWriter.Infrastructure.Integration_Tests;

/// <summary>
/// These tests are using temp folder and create few files to check that expected SUT works
/// </summary>
public class FileWatcherTests
{
    [Fact]
    public async Task FileChangedEventMustWork()
    {
        // Arrange
        var tempFolderPath = Path.GetTempPath();
        var subfolderName = "c15a7da03c274e898bddbae56d72317a";
        var testDirectory = Path.Combine(tempFolderPath, subfolderName);
        Directory.CreateDirectory(testDirectory);

        // Act
        var sut = BuildServices().GetService<IFileWatcher>();
        bool changeWasTriggered = false;
        sut!.FileChanged += (sender, args) =>
        {
            // Assert
            args.Should().NotBeNull();
            args.FullPath.Should().Be(Path.Combine(testDirectory, "test.txt"));
            changeWasTriggered = true;
        };
        sut.Start(testDirectory, "*.*", false);

        await File.WriteAllTextAsync(Path.Combine(testDirectory, "test.txt"), "test");
        await Task.Delay(TimeSpan.FromSeconds(1)); // we expect that event will be triggered during 1 second
        
        sut.Stop();
        
        if (!changeWasTriggered)
        {
            Assert.Fail("No change was triggered");
        }
      
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IFileWatcher, FileWatcher>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}