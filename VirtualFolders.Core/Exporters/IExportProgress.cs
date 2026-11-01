namespace VirtualFolders.Core.Exporters;

public interface IExportProgress
{
    void SetTotalSize(long fileSize);
    void SetCurrentFileSize(long fileSize);
    void SetCurrentFileProgress(long fileSize);
    void SetTotalFileProgress(long fileSize);
    CancellationToken CancellationToken { get; }
    void ShowDialog();
    void Close();
}
