using VirtualFolders.Core;

namespace VirtualFolders;

internal sealed class Clipboard : IClipboard
{
    public void SetText(string text)
        => System.Windows.Clipboard.SetText(text);
}
