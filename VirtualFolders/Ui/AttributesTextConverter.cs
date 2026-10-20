using System.Globalization;
using System.IO;
using System.Text;

namespace VirtualFolders.Ui;

internal sealed class AttributesTextConverter : OneWayConverter<FileAttributes, string>
{
    protected override string Convert(FileAttributes sourceValue, object parameter, CultureInfo culture)
    {
        if (sourceValue == FileAttributes.None)
            return "None";

        var sb = new StringBuilder();

        sb.Append(sourceValue.HasFlag(FileAttributes.ReadOnly) ? "R" : "-");
        sb.Append(sourceValue.HasFlag(FileAttributes.Hidden) ? "H" : "-");
        sb.Append(sourceValue.HasFlag(FileAttributes.System) ? "S" : "-");
        sb.Append(sourceValue.HasFlag(FileAttributes.Archive) ? "A" : "-");
        sb.Append(sourceValue.HasFlag(FileAttributes.Device) ? "D" : "-");
        sb.Append(sourceValue.HasFlag(FileAttributes.Normal) ? "N" : "-");
        sb.Append(sourceValue.HasFlag(FileAttributes.Temporary) ? "T" : "-");
        sb.Append(sourceValue.HasFlag(FileAttributes.Compressed) ? "C" : "-");
        sb.Append(sourceValue.HasFlag(FileAttributes.Encrypted) ? "E" : "-");

        return sb.ToString();
    }
}
