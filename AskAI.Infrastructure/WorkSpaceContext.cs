using AskAI.Infrastructure.Abstractions;

namespace AskAI.Infrastructure;

public class WorkSpaceContext(IFileSystemProvider fileSystemProvider)
    : IWorkSpaceContext
{
    private const string ApiKeyFileName = "apikey";
    
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
        tags.Remove("AskAI");
        tags.Remove(ApiKeyFileName);

        return tags.ToArray()!;
    }
}