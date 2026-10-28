using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

using Microsoft.Win32;

using VirtualFolders.Core;
using VirtualFolders.Ui;

namespace VirtualFolders;

internal sealed class Dialogs : Core.IDialogs
{
    private static OpenFileDialog Create(bool multiselect, string title, string filter)
    {
        return new OpenFileDialog
        {
            Multiselect = multiselect,
            Title = title,
            Filter = filter,
            CheckFileExists = true,
        };
    }

    public bool Confirm(string message, string title)
    {
        MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }

    public void Error(string message, string title)
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    public bool YesNo(string message, string title)
    {
        MessageBoxResult result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }

    public bool TryOpenFile(string title, string filter, [NotNullWhen(true)] out string? file)
    {
        OpenFileDialog dialog = Create(multiselect: false, title: title, filter: filter);
        bool result = dialog.ShowDialog() == true;
        file = result ? dialog.FileName : null;
        return result;
    }

    public bool TryOpenFiles(string title, string filter, [NotNullWhen(true)] out IEnumerable<string>? files)
    {
        OpenFileDialog dialog = Create(multiselect: true, title: title, filter: filter);
        bool result = dialog.ShowDialog() == true;
        files = result ? dialog.FileNames : null;
        return result;
    }

    public bool TryOpenFolder(string title, [NotNullWhen(true)] out string? folder)
    {
        var ofd = new OpenFolderDialog
        {
            Title = title,
        };
        bool result = ofd.ShowDialog() == true;
        folder = result ? ofd.SafeFolderName : null;
        return result;
    }

    public bool TrySaveFile(string title, string filter, [NotNullWhen(true)] out string? file)
    {
        var dialog = new SaveFileDialog
        {
            Title = title,
            Filter = filter,
            AddExtension = true,
        };
        bool result = dialog.ShowDialog() == true;
        file = result ? dialog.FileName : null;
        return result;
    }

    public IFileOperationProgress ShowFileOperationProgress(string title)
    {
        var dialog = new ProgressDialog
        {
            Title = title,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        return dialog;
    }

    public bool TryEditText(string title, string initialText, int maxChars, [NotNullWhen(true)] out string? result)
    {
        var dialog = new TextEditDialog(maxChars)
        {
            Title = title,
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        dialog.TextEditor.Text = initialText;
        if (dialog.ShowDialog() == true)
        {
            result = dialog.TextEditor.Text;
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    public void ShellPropertiesDialog(string file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        const uint SeeMaskInvokeIdList = 0x0000000C;
        const uint SeeMaskNoAsync = 0x00000100;

        Window? owner = Application.Current.MainWindow;

        var info = new Native.ShellExecuteInfo
        {
            Size = (uint)Marshal.SizeOf<Native.ShellExecuteInfo>(),
            Mask = SeeMaskInvokeIdList | SeeMaskNoAsync,
            Window = owner is null ? IntPtr.Zero : new WindowInteropHelper(owner).Handle,
            Verb = "properties",
            File = System.IO.Path.GetFullPath(file),
            Show = 1, // SW_SHOWNORMAL
        };

        if (!Native.ShellExecuteExW(ref info))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}