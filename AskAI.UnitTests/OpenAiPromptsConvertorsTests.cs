using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.OpenAI.Provider.Convertors;
using FluentAssertions;
using NSubstitute;

namespace AskAI.UnitTests;

public class OpenAiPromptsConvertorsTests
{
    [Fact]
    public void ToAiEntryRequest_EmptyPrompts_ReturnsEmpty()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var fileSystemLinksCollector = Substitute.For<IFileSystemLinksCollector>();
        var sut = new OpenAiPromptsConvertors(fileSystemProvider, fileSystemLinksCollector);

        // Act
        var result = sut.ToAiEntryRequest([]);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToAiEntryRequest_PromptsWithUserRole_ReturnsCorrectRequests()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var fileSystemLinksCollector = Substitute.For<IFileSystemLinksCollector>();
        fileSystemLinksCollector.Collect(Arg.Any<string>()).Returns(Enumerable.Empty<FileSystemLink>());
        var sut = new OpenAiPromptsConvertors(fileSystemProvider, fileSystemLinksCollector);

        var prompts = new List<Prompt>
        {
            new Prompt { role = ReservedKeywords.User, content = "User content" }
        };

        // Act
        var result = sut.ToAiEntryRequest(prompts);

        // Assert
        result.Should().HaveCount(1);
        result.First().role.Should().Be(ReservedKeywords.User);
        result.First().content.Should().ContainSingle(e => e["type"] == "text" && e["text"] == "User content");
    }

    [Fact]
    public void ToAiEntryRequest_PromptsWithAssistantRole_ReturnsCorrectRequests()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var fileSystemLinksCollector = Substitute.For<IFileSystemLinksCollector>();
        fileSystemLinksCollector.Collect(Arg.Any<string>()).Returns(Enumerable.Empty<FileSystemLink>());
        var sut = new OpenAiPromptsConvertors(fileSystemProvider, fileSystemLinksCollector);

        var prompts = new List<Prompt>
        {
            new Prompt { role = "assistant", content = "Assistant content" }
        };

        // Act
        var result = sut.ToAiEntryRequest(prompts);

        // Assert
        result.Should().HaveCount(1);
        result.First().role.Should().Be("assistant");
        result.First().content.Should().ContainSingle(e => e["type"] == "text" && e["text"] == "Assistant content");
    }

    [Fact]
    public void ToAiEntryRequest_PromptsWithLinks_ReturnsCorrectRequests()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var fileSystemLinksCollector = Substitute.For<IFileSystemLinksCollector>();
        var links = new List<FileSystemLink> { new FileSystemLink
            {
                Path = "link.txt",
                Key = "@link.txt"
            }
        };
        fileSystemLinksCollector.Collect(Arg.Any<string>()).Returns(links);
        fileSystemProvider.ReadAllTextAsync("link.txt").Returns(Task.FromResult("Link content"));
        var sut = new OpenAiPromptsConvertors(fileSystemProvider, fileSystemLinksCollector);

        var prompts = new List<Prompt>
        {
            new Prompt { role = ReservedKeywords.User, content = "User content with @link.txt" }
        };

        // Act
        var result = sut.ToAiEntryRequest(prompts);

        // Assert
        result.Should().HaveCount(1);
        result.First().role.Should().Be(ReservedKeywords.User);
        var content = result.First().content;

        content[0]["type"].Should().Be("text");
        content[0]["text"].Should().Be("User content with ");

        content[1]["type"].Should().Be("text");
        content[1]["text"].Should().Be("Link content");
    }

    /* TODO: uncomment this test and fix
    [Fact]
    public void ToAiEntryRequest_PromptsWithDirectory_ReturnsCorrectRequests()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var fileSystemLinksCollector = Substitute.For<IFileSystemLinksCollector>();
        var links = new List<FileSystemLink> { new FileSystemLink
            {
                Path = "directory",
                Key = "@directory"
            }
        };
        fileSystemLinksCollector.Collect(Arg.Any<string>()).Returns(links);
        fileSystemProvider.IsDirectoryExist("directory").Returns(true);
        fileSystemProvider.EnumerateFilesRecursive(["directory"]).Returns(new List<string> { "directory/file1.txt" });
        fileSystemProvider.CalculateBaseDirectory(Arg.Any<IEnumerable<string>>()).Returns("directory");
        fileSystemProvider.ReadAllTextAsync("directory/file1.txt").Returns(Task.FromResult("File content"));
        var sut = new OpenAiPromptsConvertors(fileSystemProvider, fileSystemLinksCollector);

        var prompts = new List<Prompt>
        {
            new Prompt { role = ReservedKeywords.User, content = "User content with @directory" }
        };

        // Act
        var result = sut.ToAiEntryRequest(prompts);

        // Assert
        result.Should().HaveCount(1);
        result.First().role.Should().Be(ReservedKeywords.User);
        var content = result.First().content;

        content[0]["type"].Should().Be("text");
        content[0]["text"].Should().Be("I have this folder directory, and here are details of each file:");
        
        content[1]["type"].Should().Be("text");
        content[1]["text"].Should().Be("File file1.txt, located here file1.txt with this content: ");
        
        content[2]["type"].Should().Be("text");
        content[2]["text"].Should().Be("File content");
    }
    */

    [Fact]
    public void ToAiEntryRequest_UnsupportedFileType_ThrowsNotSupportedException()
    {
        // Arrange
        var fileSystemProvider = Substitute.For<IFileSystemProvider>();
        var fileSystemLinksCollector = Substitute.For<IFileSystemLinksCollector>();
        var links = new List<FileSystemLink> { new FileSystemLink
            {
                Path = "file.unsupported",
                Key = "@file.unsupported"
            }
        };
        fileSystemLinksCollector.Collect(Arg.Any<string>()).Returns(links);
        fileSystemProvider.IsDirectoryExist("file.unsupported").Returns(false);
        var sut = new OpenAiPromptsConvertors(fileSystemProvider, fileSystemLinksCollector);

        var prompts = new List<Prompt>
        {
            new Prompt { role = ReservedKeywords.User, content = "User content with @file.unsupported" }
        };

        // Act
        Action act = () => sut.ToAiEntryRequest(prompts).ToList();

        // Assert
        act.Should().Throw<NotSupportedException>().WithMessage("File type .unsupported is not supported");
    }
}