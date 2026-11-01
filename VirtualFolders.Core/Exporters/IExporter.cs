namespace VirtualFolders.Core.Exporters;

internal interface IExporter
{
    FileSize MaxSizeLimit { get; }
    Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IExportProgress progress);
}
