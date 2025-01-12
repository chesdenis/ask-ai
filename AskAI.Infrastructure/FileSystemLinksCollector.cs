using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;

namespace AskAI.Infrastructure;

public class FileSystemLinksCollector(
    IFileSystemProvider fileSystemProvider
    ) : IFileSystemLinksCollector
{
    public IEnumerable<KeyValuePair<string, string>> Collect(string contents)
    {
        // Regular expression to match blocks starting with @
        foreach (Match match in new Regex(@"@([^\s]+)").Matches(contents))
        {
            var key = match.Groups[0].Value.Trim();
            string pathToFileOrDir = match.Groups[1].Value.Trim();

            var projectedReferences = fileSystemProvider.EnumerateFiles([pathToFileOrDir]);

            foreach (var reference in projectedReferences)
            {
                yield return new KeyValuePair<string, string>(key, reference);
            }
        }
    }
}