using System.Text.Json;

namespace VirtualFolders.Core.Format;

internal static class ProgramSettingsFile
{
    public static async Task<ProgramSettings?> LoadFrom(string fileName, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(fileName);
        return await JsonSerializer.DeserializeAsync<ProgramSettings>(
            stream,
            ProgramSettingsSerializerContext.Default.ProgramSettings,
            cancellationToken);
    }

    public static async Task SaveTo(string fileName, ProgramSettings settings, CancellationToken cancellationToken)
    {
        var newName = Path.ChangeExtension(fileName, ".new");
        using (var stream = File.Create(newName))
        {
            await JsonSerializer.SerializeAsync(
                stream,
                settings,
                ProgramSettingsSerializerContext.Default.ProgramSettings,
                cancellationToken);
        }
        File.Move(newName, fileName, overwrite: true);
    }
}