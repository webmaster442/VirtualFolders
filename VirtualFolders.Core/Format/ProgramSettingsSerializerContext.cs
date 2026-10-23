using System.Text.Json.Serialization;

namespace VirtualFolders.Core.Format;

[JsonSourceGenerationOptions(IndentSize = 2, WriteIndented =true, NumberHandling = JsonNumberHandling.WriteAsString)]
[JsonSerializable(typeof(ProgramSettings))]
internal sealed partial class ProgramSettingsSerializerContext : JsonSerializerContext
{
}