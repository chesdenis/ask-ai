using AskAI.Infrastructure.Options;
using CommandLine;

namespace AskAI.RunOptions;

public class ListenFolderFolderOptions : IListenFolderOptions
{
    [Option('w', "working-folder", Required = true, HelpText = "Path to working folder.")]
    public string WorkingFolder { get; set; }

    [Option('v', "verbose", Default = false, HelpText = "Show output (optional, default is false).")]
    public bool Verbose { get; set; }

    [Option('h', "help", HelpText = "Show help.")]
    public bool Help { get; set; }
}