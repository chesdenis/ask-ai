using AskAI.Infrastructure;
using AskAI.Infrastructure.Abstractions;
using FluentAssertions;
using NSubstitute;

namespace AskAI.UnitTests;

public class FileSystemLinksCollectorTests
{
    [Theory]
    [InlineData("@file1.txt", new[] { "file1.txt" })]
    [InlineData("@file1.txt @file2.txt", new[] { "file1.txt", "file2.txt" })]
    [InlineData("Some text @file1.txt more text", new[] { "file1.txt" })]
    [InlineData("Multiple @file1.txt lines\n@file2.txt", new[] { "file1.txt", "file2.txt" })]
    [InlineData("What you can see on this picture: @'/Downloads/2025-01-12 21.59.14.jpg'", new[] { "/Downloads/2025-01-12 21.59.14.jpg" })]
    public void Collect_ShouldReturnExpectedResults(string input, string[] expectedOutput)
    {
        // Arrange
        var sut = new FileSystemLinksCollector();

        // Act
        var result = sut.Collect(input).ToArray();

        // Assert
        result.Select(s=>s.Value).Should().BeEquivalentTo(expectedOutput);
    }
    
    
}