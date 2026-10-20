namespace VirtualFolders.Core;

public interface IFileOperationProgress
{
    void SetTotalSize(long fileSize);
    void SetCurrentFileSize(long fileSize);
    void SetCurrentFileProgress(long fileSize);
    void SetTotalFileProgress(long fileSize);
    CancellationToken CancellationToken { get; }
    void ShowDialog();
    void Close();
}
