namespace AITextWriter.Infrastructure.Abstractions;

public interface IFileSystemNotifier
{
    event EventHandler<FileSystemEventArgs> FileChanged;
    void Start(string path, string filter, bool recursive);
    void Stop();
}