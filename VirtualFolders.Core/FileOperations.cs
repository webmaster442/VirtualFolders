using System.Buffers;
using System.IO.Compression;

namespace VirtualFolders.Core;

internal static class FileOperations
{
    public static async Task CopyVirtualFolderToDirectory(VirtualFolder folder, string destinationDirectory, IFileOperationProgress progress)
    {
        FileOperationState state = new(folder.Files.Sum(x => x.FileSize.TotalBytes));
        state.Initialize(progress);

        byte[] buffer = ArrayPool<byte>.Shared.Rent(16 * 1024);
        int read = 0;

        progress.ShowDialog();

        try
        {
            foreach (VirtualFile file in folder.Files)
            {
                progress.CancellationToken.ThrowIfCancellationRequested();
                state.NewFile(file.FileSize, progress);

                using (FileStream readStream = File.OpenRead(file.FullPath))
                {
                    string destinationFilePath = Path.Combine(destinationDirectory, Path.ChangeExtension(file.Name, file.Extension));
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

    public static async Task CreateZipFromVirtualFolder(VirtualFolder folder, string destinationFile, IFileOperationProgress progress)
    {
        FileOperationState state = new(folder.Files.Sum(x => x.FileSize.TotalBytes));
        state.Initialize(progress);

        byte[] buffer = ArrayPool<byte>.Shared.Rent(16 * 1024);
        int read = 0;

        progress.ShowDialog();

        try
        {
            using (FileStream targetStream = File.Create(destinationFile))
            {
                using (var archive = new ZipArchive(targetStream, ZipArchiveMode.Create))
                {
                    foreach (VirtualFile file in folder.Files)
                    {
                        progress.CancellationToken.ThrowIfCancellationRequested();
                        state.NewFile(file.FileSize, progress);

                        var entry = archive.CreateEntry(Path.ChangeExtension(file.Name, file.Extension), CompressionLevel.Optimal);
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

    public static async Task ExportM3U(VirtualFolder folder, string destinationFile, IFileOperationProgress progress)
    {
        FileOperationState state = new(folder.Files.Count);
        state.Initialize(progress);

        progress.ShowDialog();

        try
        {
            using (FileStream targetStream = File.Create(destinationFile))
            {
                state.NewFile(folder.Files.Count, progress);
                using (StreamWriter writer = new(targetStream))
                {
                    foreach (VirtualFile file in folder.Files)
                    {
                        progress.CancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync(file.Name, progress.CancellationToken);
                        state.Update(1, progress);
                    }
                }
            }
        }
        finally
        {
            state.Finished(progress);
            progress.Close();
        }
    }

    public static async Task ExportPls(VirtualFolder folder, string destinationFile, IFileOperationProgress progress)
    {
        FileOperationState state = new(folder.Files.Count);
        state.Initialize(progress);
        progress.ShowDialog();

        try
        {
            using (FileStream targetStream = File.Create(destinationFile))
            {
                state.NewFile(folder.Files.Count, progress);
                int index = 1;
                using (StreamWriter writer = new(targetStream))
                {
                    await writer.WriteLineAsync("[playlist]", progress.CancellationToken);
                    foreach (VirtualFile file in folder.Files)
                    {
                        progress.CancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync($"File{index}={file.FullPath}", progress.CancellationToken);
                        index++;
                        state.Update(1, progress);

                    }
                    await writer.WriteLineAsync($"NumberOfEntries={index - 1}", progress.CancellationToken);
                }
            }
        }
        finally
        {
            state.Finished(progress);
            progress.Close();
        }
    }
}