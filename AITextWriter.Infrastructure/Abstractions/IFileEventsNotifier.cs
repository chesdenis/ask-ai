namespace AITextWriter.Infrastructure.Abstractions;

public interface IFileEventsNotifier
{
    event EventHandler<FileSystemEventArgs> FileChanged;
    void Start(string path, string filter = "*.*", bool recursive = true);
    void Stop();
}