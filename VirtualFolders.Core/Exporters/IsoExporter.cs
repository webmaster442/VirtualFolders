using DiscUtils.Udf;

namespace VirtualFolders.Core.Exporters;

internal sealed class IsoExporter : IIndeterminateExporter
{
    private readonly string _volumeIdentifier;

    public IsoExporter(string volumeIdentifier)
    {
        _volumeIdentifier = volumeIdentifier;
    }

    public FileSize MaxSizeLimit => FileSize.GigaByte * 25;

    public async Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IIndeterminateExportProgress progress)
    {
        progress.ShowDialog();

        try
        {
            var udfBuilder = new UdfBuilder
            {
                VolumeIdentifier = _volumeIdentifier
            };
            foreach (var file in folder)
            {
                if (file.IsUrl)
                {
                    udfBuilder.AddFile($"{file.Name}.url", file.WriteAsBytes());
                }
                else
                {
                    udfBuilder.AddFile($"{file.Name}.{file.Extension}", file.FullPath);
                }
            }

            using var stream = File.Create(destination);
            await udfBuilder.BuildAsync(stream, progress.CancellationToken);
        }
        finally
        {
            progress.Finished();
            progress.Close();
        }
    }
}
