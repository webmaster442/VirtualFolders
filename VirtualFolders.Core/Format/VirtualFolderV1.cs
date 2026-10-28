using System.Text.Json.Serialization;

namespace VirtualFolders.Core.Format;

internal sealed record class VirtualFolderV1
{
    [JsonPropertyName("version")]
    public required string Version { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("lastModified")]
    public required DateTime LastModified { get; init; }

    [JsonPropertyName("files")]
    public required VirtualFileV1[] Files { get; init; }

    [JsonPropertyName("notes")]
    public required string Notes { get; init; }
}
