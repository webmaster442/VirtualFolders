namespace VirtualFolders.Core.Exporters;

internal sealed class M3UExporter : IExporter
{
    public FileSize MaxSizeLimit => FileSize.MaxValue;

    public async Task Execute(IReadOnlyList<VirtualFile> folder, string destination, IExportProgress progress)
    {
        ExportState state = new(folder.Count);
        state.Initialize(progress);

        progress.ShowDialog();

        try
        {
            using (FileStream targetStream = File.Create(destination))
            {
                state.NewFile(folder.Count, progress);
                using (StreamWriter writer = new(targetStream))
                {
                    foreach (VirtualFile file in folder)
                    {
                        progress.CancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync(file.FullPath);
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
}