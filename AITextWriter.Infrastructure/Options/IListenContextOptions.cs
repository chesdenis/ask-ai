namespace AITextWriter.Infrastructure.Options;

public interface IListenContextOptions
{
    string WorkingFolder { get; set; }
    string Model { get; set; }
    bool Verbose { get; set; }
    bool Help { get; set; }
}