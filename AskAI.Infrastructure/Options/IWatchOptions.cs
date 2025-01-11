namespace AskAI.Infrastructure.Options;

public interface IWatchOptions
{
    string WorkingFolder { get; set; }
    bool Verbose { get; set; }
    bool Help { get; set; }
}