namespace VirtualFolders.Core.Exporters;

internal interface IIndeterminateExporter : IHasSizeLimit
{
    Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IIndeterminateExportProgress progress);
}