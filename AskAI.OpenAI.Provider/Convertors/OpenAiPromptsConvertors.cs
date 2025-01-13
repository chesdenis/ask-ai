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
            .Select(s => new AiPromptRequest
            {
                role = s.role,
                content = ToContent(s.links, s.content).ToArray()
            });
    }

    private IEnumerable<AiPromptEntryRequest> ToContent(IEnumerable<KeyValuePair<string, string>> links, string content)
    {
        var enrichedContent = new List<AiPromptEntryRequest>();
        int currentIndex = 0;

        foreach (var link in links)
        {
            int linkIndex = content.IndexOf(link.Key, currentIndex, StringComparison.Ordinal);
            if (linkIndex == -1) continue;

            if (linkIndex > currentIndex)
            {
                enrichedContent.Add(new AiPromptEntryRequest
                {
                    { "type", "text" },
                    { "text", content.Substring(currentIndex, linkIndex - currentIndex) }
                });
            }

            enrichedContent.Add(ToLinkEntryRequest(link));

            currentIndex = linkIndex + link.Key.Length;
        }

        if (currentIndex < content.Length)
        {
            enrichedContent.Add(new AiPromptEntryRequest
            {
                { "type", "text" },
                { "text", content[currentIndex..] }
            });
        }

        return enrichedContent;
    }

    private AiPromptEntryRequest ToLinkEntryRequest(KeyValuePair<string, string> link)
    {
        switch (Path.GetExtension(link.Value).ToLowerInvariant())
        {
            case ".jpg":
            case ".jpeg":
                return new AiPromptEntryRequest()
                {
                    { "type", "image_url" },
                    {
                        "image_url",
                        new Dictionary<string, string>()
                            { { "url", $"data:image/jpeg;base64,{fileSystemProvider.EncodeAsBase64(link.Value)}" } }
                    }
                };
            default:
                throw new NotSupportedException($"File type {Path.GetExtension(link.Value)} is not supported");
        }
    }
}