using System.Buffers;
using System.IO.Compression;

namespace VirtualFolders.Core.Exporters;

internal sealed class ZipExporter : IExporter
{
    public async Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IExportProgress progress)
    {
        ExportState state = new(folder.Sum(x => x.FileSize.TotalBytes));
        state.Initialize(progress);

        byte[] buffer = ArrayPool<byte>.Shared.Rent(16 * 1024);
        int read = 0;

        progress.ShowDialog();

        try
        {
            using (FileStream targetStream = File.Create(destination))
            {
                using (var archive = new ZipArchive(targetStream, ZipArchiveMode.Create))
                {
                    foreach (VirtualFile file in folder)
                    {
                        if (file.IsUrl)
                        {
                            var urlEntry = archive.CreateEntry(file.NameWithExtension(".url"), CompressionLevel.Optimal);
                            using var urlStream = await urlEntry.OpenAsync(progress.CancellationToken);
                            await file.WriteAsUrlFileAsync(urlStream, progress.CancellationToken);
                            continue;
                        }

                        progress.CancellationToken.ThrowIfCancellationRequested();
                        state.NewFile(file.FileSize, progress);

                        var entry = archive.CreateEntry(file.NameWithExtension(), CompressionLevel.Optimal);
                        using (var writeStream = await entry.OpenAsync(progress.CancellationToken))
                        {
                            using (var readStream = File.OpenRead(file.FullPath))
                            {
                                do
                                {
                                    read = await readStream.ReadAsync(buffer, 0, buffer.Length, progress.CancellationToken);
                                    await writeStream.WriteAsync(buffer, 0, read, progress.CancellationToken);
                                    state.Update(read, progress);
                                }
                                while (read > 0);
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            state.Finished(progress);
            progress.Close();
        }
    }
}
