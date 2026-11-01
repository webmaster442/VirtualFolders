namespace VirtualFolders.Core.Exporters;

internal sealed class PlsExporter : IExporter
{
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
                int index = 1;
                using (StreamWriter writer = new(targetStream))
                {
                    await writer.WriteLineAsync("[playlist]");
                    foreach (VirtualFile file in folder)
                    {
                        progress.CancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync($"File{index}={file.FullPath}");
                        index++;
                        state.Update(1, progress);

                    }
                    await writer.WriteLineAsync($"NumberOfEntries={index - 1}");
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
