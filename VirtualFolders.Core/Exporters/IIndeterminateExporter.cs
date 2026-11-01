namespace VirtualFolders.Core.Exporters;

internal interface IIndeterminateExporter
{
    Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IIndeterminateExportProgress progress);
}