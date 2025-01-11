namespace AskAI.Infrastructure.Options;

public interface IListenFolderOptions
{
    string WorkingFolder { get; set; }
    bool Verbose { get; set; }
    bool Help { get; set; }
}