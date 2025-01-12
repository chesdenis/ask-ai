using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;
using AskAI.Services.Abstractions;

namespace AskAI.Services;

public class PromptLinksCollector(IWorkSpaceContext workSpaceContext) : IPromptLinksCollector
{
    public IEnumerable<string> Collect(string contents)
    {
        // Regular expression to match blocks starting with @
        foreach (Match match in new Regex(@"@([^\s]+)").Matches(contents))
        {
            string pathToFileOrDir = match.Groups[1].Value.Trim();
            
            var projectedReferences = CollectNested(pathToFileOrDir).ToArray();

            foreach (var reference in projectedReferences)
            {
                yield return reference;
            }
        }
    }

    private IEnumerable<string> CollectNested(string path)
    {
        var isDirectory = Directory.Exists(path);

        if (isDirectory)
        {
            var files = Directory.EnumerateFiles(path, "*.*", 
                SearchOption.AllDirectories).ToArray();

            foreach (var f in files)
            {
                yield return f;
            }
        }
            
        yield return path;
    }
}