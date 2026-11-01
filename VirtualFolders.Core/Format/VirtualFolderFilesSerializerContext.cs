using System.Text.Json.Serialization;

namespace VirtualFolders.Core.Format;

[JsonSourceGenerationOptions(IndentSize = 2,
                             WriteIndented =true,
                             NumberHandling = JsonNumberHandling.WriteAsString, 
                             DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(VirtualFolderV1))]
internal sealed partial class VirtualFolderFilesSerializerContext : JsonSerializerContext
{

}