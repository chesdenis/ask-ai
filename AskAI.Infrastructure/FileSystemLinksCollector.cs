using System.Text.RegularExpressions;
using AskAI.Infrastructure.Abstractions;
using AskAI.Model;

namespace AskAI.Infrastructure;

public class FileSystemLinksCollector : IFileSystemLinksCollector
{
    public IEnumerable<FileSystemLink> Collect(string contents)
    {
        var patterns = new[]
        {
            @"@([^\s'""]+)",          // Matches @filePath
            @"@'([^']+)'",            // Matches @'filePath1'
            @"@""([^""]+)"""          // Matches @\"filePath2\"
        };
        
        foreach (var pattern in patterns)
        foreach (Match match in new Regex(pattern).Matches(contents))
        {
            var key = match.Groups[0].Value.Trim();
            string pathToFsLink = match.Groups[1].Value.Trim();
            
            yield return new FileSystemLink
            {
                Path = pathToFsLink,
                Key = key
            };
        }
    }
}