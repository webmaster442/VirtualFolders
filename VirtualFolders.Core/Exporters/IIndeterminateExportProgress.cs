namespace VirtualFolders.Core.Exporters;

public interface IIndeterminateExportProgress
{
    CancellationToken CancellationToken { get; }
    void Finished();
    void ShowDialog();
    void Close();
}