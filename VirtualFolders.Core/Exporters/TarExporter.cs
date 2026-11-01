using System.Formats.Tar;

namespace VirtualFolders.Core.Exporters;

internal sealed class TarExporter : IIndeterminateExporter
{
    public FileSize MaxSizeLimit => FileSize.GigaByte * 100;

    public async Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IIndeterminateExportProgress progress)
    {
        progress.ShowDialog();

        try
        {
            using (FileStream targetStream = File.Create(destination))
            {
                TarWriter writer = new TarWriter(targetStream);
                foreach (var file in folder)
                {
                    if (file.IsUrl)
                    {
                        var entry = new GnuTarEntry(TarEntryType.RegularFile, file.NameWithExtension(".url"));
                        using var stream = new MemoryStream();
                        await file.WriteAsUrlFileAsync(stream, progress.CancellationToken);
                        stream.Position = 0;
                        entry.DataStream = stream;
                        await writer.WriteEntryAsync(entry, progress.CancellationToken);
                        continue;
                    }

                    progress.CancellationToken.ThrowIfCancellationRequested();
                    await writer.WriteEntryAsync(file.FullPath, file.NameWithExtension(), progress.CancellationToken);
                }
            }
        }
        finally
        {
            progress.Finished();
            progress.Close();
        }

    }
}
