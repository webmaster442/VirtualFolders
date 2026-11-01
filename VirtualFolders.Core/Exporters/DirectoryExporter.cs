using System.Buffers;

namespace VirtualFolders.Core.Exporters;

internal sealed class DirectoryExporter : IExporter
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
            foreach (VirtualFile file in folder)
            {
                progress.CancellationToken.ThrowIfCancellationRequested();
                state.NewFile(file.FileSize, progress);

                if (file.IsUrl)
                {
                    using var urlFile = File.Create(Path.Combine(destination, file.NameWithExtension(".url")));
                    await file.WriteAsUrlFileAsync(urlFile, progress.CancellationToken);
                    continue;
                }

                using (FileStream readStream = File.OpenRead(file.FullPath))
                {
                    string destinationFilePath = Path.Combine(destination, file.NameWithExtension());
                    using (FileStream writeStream = File.Create(destinationFilePath))
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
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            state.Finished(progress);
            progress.Close();
        }
    }
}