namespace VirtualFolders.Core.Exporters;

internal interface IHasSizeLimit
{
    FileSize MaxSizeLimit { get; }
}
