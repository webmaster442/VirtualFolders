using System.Text.Json;

namespace VirtualFolders.Core.Format;

internal static class VirtualFolderFile
{
    public static async Task<VirtualFolder?> LoadFrom(string fileName, IDialogs dialogs, IClipboard clipboard, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(fileName);

        VirtualFolderV1? folder = await JsonSerializer.DeserializeAsync<VirtualFolderV1>(
            stream,
            VirtualFolderFilesSerializerContext.Default.VirtualFolderV1,
            cancellationToken);

        if (folder == null)
            return null;

        VirtualFolder result = new(fileName, dialogs, clipboard);

        result.Notes = folder.Notes.Length > FormatLimits.FolderNotes
            ? folder.Notes[..FormatLimits.FolderNotes]
            : folder.Notes;

        foreach (var file in folder.Files)
        {
            result.Files.Add(VirtualFile.CreateFromFilePath(file.Path, file.Added.ToLocalTime()));
        }

        return result;
    }

    public static async Task SaveTo(string fileName, VirtualFolder virtualFolder, CancellationToken cancellationToken)
    {
        var newName = Path.ChangeExtension(fileName, ".new");
        using (var stream = File.Create(newName))
        {
            VirtualFolderV1 result = new VirtualFolderV1
            {
                Name = virtualFolder.Name,
                Version = "1.0",
                Notes = virtualFolder.Notes,
                LastModified = virtualFolder.LastModified,
                Files = virtualFolder.Files.Select(x => new VirtualFileV1
                {
                    Path = x.FullPath,
                    Added = x.Added,
                    Notes = x.Notes.Length > FormatLimits.FileNotes
                        ? x.Notes[..FormatLimits.FileNotes]
                        : x.Notes
                }).ToArray()
            };

            await JsonSerializer.SerializeAsync(
                stream,
                result,
                VirtualFolderFilesSerializerContext.Default.VirtualFolderV1,
                cancellationToken);
        }
        File.Move(newName, fileName, overwrite: true);
    }
}