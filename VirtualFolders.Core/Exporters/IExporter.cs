namespace VirtualFolders.Core.Exporters;

internal interface IExporter : IHasSizeLimit
{
    Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IExportProgress progress);
}
