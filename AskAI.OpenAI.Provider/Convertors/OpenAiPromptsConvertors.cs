using AskAI.Infrastructure.Abstractions;
using AskAI.Model;
using AskAI.OpenAI.Provider.Abstraction;
using AskAI.OpenAI.Provider.Request;

namespace AskAI.OpenAI.Provider.Convertors;

public class OpenAiPromptsConvertors(
    IFileSystemProvider fileSystemProvider,
    IFileSystemLinksCollector fileSystemLinksCollector) : IOpenAiPromptsConvertors
{
    public IEnumerable<AiPromptRequest> ToAiEntryRequest(IEnumerable<Prompt> prompts)
    {
        return prompts.Select(p => new
            {
                p.role,
                links = fileSystemLinksCollector.Collect(p.content),
                p.content
            })
            .Select(s =>
            {
                if(s.role == ReservedKeywords.User)
                {
                    return new AiPromptRequest
                    {
                        role = s.role,
                        content = InjectLinksIntoContent(s.links, s.content).ToArray()
                    };
                }

                return new AiPromptRequest()
                {
                    role = s.role,
                    content =
                    [
                        new AiPromptEntryRequest
                        {
                            { "type", "text" },
                            { "text", s.content }
                        }
                    ]
                };

            });
    }

    private IEnumerable<AiPromptEntryRequest> InjectLinksIntoContent(IEnumerable<FileSystemLink> links, string content)
    {
        var result = new List<AiPromptEntryRequest>();
        int currentIndex = 0;

        foreach (var link in links)
        {
            var linkIndex = content.IndexOf(link.Key, currentIndex, StringComparison.Ordinal);
            if (linkIndex == -1) continue;

            if (linkIndex > currentIndex)
            {
                result.Add(new AiPromptEntryRequest
                {
                    { "type", "text" },
                    { "text", content.Substring(currentIndex, linkIndex - currentIndex) }
                });
            }

            var linkExpandedContent = ProcessLink(link);

            result.AddRange(linkExpandedContent);

            currentIndex = linkIndex + link.Key.Length;
        }

        if (currentIndex < content.Length)
        {
            result.Add(new AiPromptEntryRequest
            {
                { "type", "text" },
                { "text", content[currentIndex..] }
            });
        }

        return result;
    }

    private IEnumerable<AiPromptEntryRequest> ProcessLink(FileSystemLink link)
    {
        var isDirectory = fileSystemProvider.IsDirectoryExist(link.Path);

        return isDirectory ? ProcessAsDirectory(link) : [ProcessPath(link.Path)];
    }
    
    private IEnumerable<AiPromptEntryRequest> ProcessAsDirectory(FileSystemLink link)
    {
        var files = fileSystemProvider.EnumerateFilesRecursive([link.Path]);
        var baseDirectory = fileSystemProvider.CalculateBaseDirectory(files);

        yield return new AiPromptEntryRequest
        {
            { "type", "text" },
            { "text", $"I have this folder {baseDirectory}, and here are details of each file:" }
        };

        foreach (var filePath in files)
        {
            yield return new AiPromptEntryRequest()
            {
                { "type", "text" },
                { "text", $"File {Path.GetFileName(filePath)}, " +
                          $"located here {Path.GetRelativePath(baseDirectory,filePath)} with this content: " }
            };
            yield return ProcessPath(filePath);
        }
    }
    
    private AiPromptEntryRequest ProcessPath(string path)
    {
        switch (Path.GetExtension(path).ToLowerInvariant())
        {
            case ".jpg":
            case ".jpeg":
                return new AiPromptEntryRequest
                {
                    { "type", "image_url" },
                    {
                        "image_url",
                        new Dictionary<string, string>()
                            { { "url", $"data:image/jpeg;base64,{fileSystemProvider.EncodeAsBase64(path)}" } }
                    }
                };
            case ".md":
            case ".cs":
            case ".txt":
            case ".json":
            case ".xml":
            case ".yaml":
            case ".js":
            case ".ts":
                return new AiPromptEntryRequest
                {
                    { "type", "text" },
                    { "text", fileSystemProvider.ReadAllTextAsync(path)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult() }
                };
            default:
                throw new NotSupportedException($"File type {Path.GetExtension(path)} is not supported");
        }
    }
}