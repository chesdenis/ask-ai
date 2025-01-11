using AskAI.Infrastructure.Abstractions;

namespace AskAI.Infrastructure;

public class FileEventsNotifier : IFileEventsNotifier
{
    private FileSystemWatcher? _fileSystemWatcher;

    public event EventHandler<FileSystemEventArgs>? FileChanged;

    /// <summary>
    /// Start listen of all updates in folder.
    /// </summary>
    /// <param name="path">directory to monitor</param>
    /// <param name="filter">filter for files to listen</param>
    /// <param name="recursive">recursive listen</param>
    public void Start(string path, string filter = "*.*", bool recursive = true)
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
        if (_fileSystemWatcher == null) return;
        
        _fileSystemWatcher.EnableRaisingEvents = false;
        _fileSystemWatcher.Changed -= OnChanged;
        _fileSystemWatcher.Created -= OnChanged;
        _fileSystemWatcher.Deleted -= OnChanged;
        _fileSystemWatcher.Renamed -= OnRenamed;
        _fileSystemWatcher.Dispose();
        _fileSystemWatcher = null;
    }

    private void OnChanged(object sender, FileSystemEventArgs e) => FileChanged?.Invoke(this, e);

    private void OnRenamed(object sender, RenamedEventArgs e) => FileChanged?.Invoke(this, e);
}