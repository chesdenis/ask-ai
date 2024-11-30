using AITextWriter.Infrastructure.Abstractions;

namespace AITextWriter.Infrastructure;

public class FileSystemNotifier : IFileSystemNotifier
{
    private FileSystemWatcher _fileSystemWatcher;

    public event EventHandler<FileSystemEventArgs> FileChanged;

    public void Start(string path, string filter, bool recursive)
    {
        _fileSystemWatcher = new FileSystemWatcher(path, filter)
        {
            IncludeSubdirectories = recursive,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };

        _fileSystemWatcher.Changed += OnChanged;
        _fileSystemWatcher.Created += OnChanged;
        _fileSystemWatcher.Deleted += OnChanged;
        _fileSystemWatcher.Renamed += OnRenamed;

        _fileSystemWatcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        if (_fileSystemWatcher != null)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            _fileSystemWatcher.Changed -= OnChanged;
            _fileSystemWatcher.Created -= OnChanged;
            _fileSystemWatcher.Deleted -= OnChanged;
            _fileSystemWatcher.Renamed -= OnRenamed;
            _fileSystemWatcher.Dispose();
            _fileSystemWatcher = null;
        }
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        FileChanged?.Invoke(this, e);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        FileChanged?.Invoke(this, e);
    }
}