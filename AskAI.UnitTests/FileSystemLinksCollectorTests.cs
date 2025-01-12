using AskAI.Infrastructure;
using AskAI.Infrastructure.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace AskAI.UnitTests;

public class FileSystemLinksCollectorTests
{
    [Theory]
    [InlineData("@file1.txt", new[] { "file1.txt" })]
    [InlineData("@dir1", new[] { "dir1/file1.txt", "dir1/file2.txt" })]
    [InlineData("@file1.txt @file2.txt", new[] { "file1.txt", "file2.txt" })]
    [InlineData("@dir1 @file1.txt", new[] { "dir1/file1.txt", "dir1/file2.txt", "file1.txt" })]
    [InlineData("Some text @file1.txt more text", new[] { "file1.txt" })]
    [InlineData("Multiple @file1.txt lines\n@file2.txt", new[] { "file1.txt", "file2.txt" })]
    [InlineData("Text with @dir1\nand @file1.txt", new[] { "dir1/file1.txt", "dir1/file2.txt", "file1.txt" })]
    [InlineData("Mixed @dir1 text @file1.txt\n@file2.txt", new[] { "dir1/file1.txt", "dir1/file2.txt", "file1.txt", "file2.txt" })]
    public void Collect_ShouldReturnExpectedResults(string input, string[] expectedOutput)
    {
        // Arrange
        var mockFileSystemProvider = Substitute.For<IFileSystemProvider>();
        mockFileSystemProvider.EnumerateFiles(Arg.Any<IEnumerable<string>>())
            .Returns(callInfo => 
            {
                var paths = callInfo.Arg<IEnumerable<string>>();
                return paths.SelectMany(path => path == "dir1" 
                    ? new[] { "dir1/file1.txt", "dir1/file2.txt" } 
                    : new[] { path });
            });

        var sut = new FileSystemLinksCollector(mockFileSystemProvider);

        // Act
        var result = sut.Collect(input).ToArray();

        // Assert
        result.Select(s=>s.Value).Should().BeEquivalentTo(expectedOutput);
    }
    
    
}