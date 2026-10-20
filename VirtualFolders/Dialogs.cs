using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
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
}