using AITextWriter.Infrastructure.Options;
using CommandLine;

public class Options : IListenContextOptions
{
    [Option('w', "working-folder", Required = true, HelpText = "Path to working folder.")]
    public string WorkingFolder { get; set; }

    [Option('m', "model", Default = "", HelpText = "Model name (optional, if empty will be used default depending on LLM api).")]
    public string Model { get; set; }

    [Option('v', "verbose", Default = false, HelpText = "Show output (optional, default is false).")]
    public bool Verbose { get; set; }

    [Option('h', "help", HelpText = "Show help.")]
    public bool Help { get; set; }
}