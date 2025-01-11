using AskAI.Infrastructure;
using AskAI.Infrastructure.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AITextWriter.Infrastructure.Integration_Tests;

/// <summary>
/// These tests are using temp folder and create few files to check that expected SUT works
/// </summary>
public class FileSystemProviderTests
{
    [Theory]
    [InlineData("*.", 1)]
    [InlineData("*.*", 2)]
    [InlineData("*.test", 1)]
    [InlineData("*9f56cb9f5ceb4898b641b0595c7f3055.test", 1)]
    [InlineData("*9missing.test", 0)]
    public async Task GetFilePathsAsync_CanSelectFilesWithDifferentPatterns(string pattern, int expectedCount)
    {
        // Arrange
        var tempFolderPath = Path.GetTempPath();
        var fileName = "9f56cb9f5ceb4898b641b0595c7f3055";
        var extension = ".test";
        var filePathWithExtension = Path.Combine(tempFolderPath, fileName + extension);
        var filePathWithoutExtension = Path.Combine(tempFolderPath, fileName);

        await File.WriteAllTextAsync(filePathWithExtension, "test");
        await File.WriteAllTextAsync(filePathWithoutExtension, "test");

        // Act
        var sut = BuildServices().GetService<IFileSystemProvider>();
        var result = await sut!.GetFilePathsAsync(tempFolderPath, pattern, false);
        result = result.Select(Path.GetFileName).ToArray();
        result = result.Where(s => s.StartsWith(fileName)).ToArray();

        // Assert
        result.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task ReadAllTextAsync_MustGetAllTextData()
    {
        // Arrange
        var tempFolderPath = Path.GetTempPath();
        var fileName = "9f56cb9f5ceb4898b641b0595c7f3055";
        var extension = ".test";
        var filePathWithExtension = Path.Combine(tempFolderPath, fileName + extension);

        await File.WriteAllTextAsync(filePathWithExtension, "test");

        // Act
        var sut = BuildServices().GetService<IFileSystemProvider>();
        var result = await sut.ReadAllTextAsync(filePathWithExtension);

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public async Task WriteAllTextAsync_MustGetAllTextData()
    {
        // Arrange
        var tempFolderPath = Path.GetTempPath();
        var fileName = "3c579c7aadbf427590e384cf31dfb454";
        var extension = ".test";
        var filePathWithExtension = Path.Combine(tempFolderPath, fileName + extension);

        await File.WriteAllTextAsync(filePathWithExtension, "write-text");

        // Act
        var sut = BuildServices().GetService<IFileSystemProvider>();
        await sut.WriteAllTextAsync(filePathWithExtension, "another-text");

        var result = await sut.ReadAllTextAsync(filePathWithExtension);

        // Assert
        result.Should().Be("another-text");
    }

    private ServiceProvider BuildServices(Func<ServiceCollection, ServiceCollection>? factory = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IFileSystemProvider, FileSystemProvider>();
        serviceCollection.AddLogging();
        serviceCollection = factory?.Invoke(serviceCollection) ?? serviceCollection;
        return serviceCollection.BuildServiceProvider();
    }
}