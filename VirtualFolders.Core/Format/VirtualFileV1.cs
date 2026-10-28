using System.Text.Json.Serialization;

namespace VirtualFolders.Core.Format;

internal sealed record class VirtualFileV1
{
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    [JsonPropertyName("added")]
    public required DateTime Added { get; init; }

    [JsonPropertyName("notes")]
    public required string Notes { get; init; }
}
