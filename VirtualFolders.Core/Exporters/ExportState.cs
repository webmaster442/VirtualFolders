namespace VirtualFolders.Core.Exporters;

internal sealed class ExportState
{
    private readonly long _totalFilesSize;
    private long _currentFileSize;
    private long _totalProcessed;
    private long _currentFileProcessed;
    private long _onePercent = 0;
    private long _lastReportedCurrentFileProcessed = 0;

    private DateTime _lastUpdate;

    public ExportState(long totalFilesSize)
    {
        _lastUpdate = DateTime.MinValue;
        _totalFilesSize = totalFilesSize;
        _currentFileSize = 0;
    }

    public void Initialize(IExportProgress progress)
    {
        progress.SetTotalSize(_totalFilesSize);
        progress.SetCurrentFileProgress(0);
        progress.SetTotalFileProgress(0);
    }

    public void NewFile(FileSize fileSize, IExportProgress progress)
    {
        _currentFileProcessed = 0;
        _currentFileSize = fileSize.TotalBytes;
        _onePercent = fileSize.TotalBytes / 100;
        _lastReportedCurrentFileProcessed = 0;
        progress.SetCurrentFileSize(fileSize.TotalBytes);
        progress.SetCurrentFileProgress(0);
        _lastUpdate = DateTime.Now;
    }

    public void Update(int read, IExportProgress progress)
    {
        _currentFileProcessed += read;
        _totalProcessed += read;
        var datediff = (DateTime.Now - _lastUpdate).TotalSeconds;
        if (_currentFileProcessed - _lastReportedCurrentFileProcessed >= _onePercent
            || datediff > 1)
        {
            progress.SetCurrentFileProgress(_currentFileProcessed);
            progress.SetTotalFileProgress(_totalProcessed);
            _lastReportedCurrentFileProcessed = _currentFileProcessed;
            _lastUpdate = DateTime.Now;
        }
    }

    public void Finished(IExportProgress progress)
    {
        _currentFileProcessed = _currentFileSize;
        _totalProcessed = _totalFilesSize;
        progress.SetCurrentFileProgress(_currentFileProcessed);
        progress.SetTotalFileProgress(_totalProcessed);
    }
}
