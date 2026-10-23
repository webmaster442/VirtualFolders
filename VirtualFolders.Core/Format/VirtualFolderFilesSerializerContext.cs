using System.Text.Json.Serialization;

namespace VirtualFolders.Core.Format;

[JsonSourceGenerationOptions(IndentSize = 2, WriteIndented =true, NumberHandling = JsonNumberHandling.WriteAsString)]
[JsonSerializable(typeof(VirtualFolderV1))]
internal sealed partial class VirtualFolderFilesSerializerContext : JsonSerializerContext
{

}