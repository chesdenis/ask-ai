using AskAI.Infrastructure.Abstractions;
using AskAI.Model;

namespace AskAI.Infrastructure;

public class WorkSpaceContext(IFileSystemProvider fileSystemProvider)
    : IWorkSpaceContext
{
    public async Task<string[]> GetTagsAsync(string filePath)
    {
        var workingPath = Path.GetDirectoryName(filePath);
        var files = await fileSystemProvider.GetFilePathsAsync(
            workingPath,
            "*.", false);

        var tags = files.Select(Path.GetFileNameWithoutExtension).ToList();
        
        // this is target file, and we need to exclude it from tags
        tags.Remove(Path.GetFileNameWithoutExtension(filePath)); 

        // these are generator file if available (on MacOs/Linux can be)
        tags.Remove(ReservedKeywords.TimeoutMinutes);
        tags.Remove(ReservedKeywords.Endpoint);
        tags.Remove(ReservedKeywords.ApiKey);
        tags.Remove(ReservedKeywords.AskAI);
        return tags.ToArray()!;
    }
}