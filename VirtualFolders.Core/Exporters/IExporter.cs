namespace VirtualFolders.Core.Exporters;

internal interface IExporter
{
    Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IExportProgress progress);
}
