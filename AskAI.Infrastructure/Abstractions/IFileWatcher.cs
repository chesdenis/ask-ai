namespace AskAI.Infrastructure.Abstractions;

public interface IFileWatcher
{
    event EventHandler<FileSystemEventArgs> FileChanged;
    void Start(string path, string filter = "*.*", bool recursive = true);
    void Stop();
}