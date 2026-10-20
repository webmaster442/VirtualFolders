using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace VirtualFolders.Ui;

internal sealed class IsAvailableConverter : OneWayConverter<bool, TextDecorationCollection>
{
    protected override TextDecorationCollection Convert(bool sourceValue, object parameter, CultureInfo culture)
    {
        return sourceValue 
            ? new TextDecorationCollection()
            : TextDecorations.Strikethrough;
    }
}