using System.Collections.Specialized;
using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using VirtualFolders.Core.Format;
using VirtualFolders.Core.Properties;

namespace VirtualFolders.Core;

public sealed partial class VirtualFolder : ObservableObject
{
    private readonly string _fileName;

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial DateTime LastModified { get; set; }

    public BulkObservableCollection<VirtualFile> SelectedItems { get; set; }

    private bool HasSelectedItem() 
        => SelectedItems.Count > 0;

    [ObservableProperty]
    public partial FileSize TotalSize { get; private set; }

    public BulkObservableCollection<VirtualFile> Files { get; }

    private readonly IDialogs _dialogs;
    private readonly IClipboard _clipboard;

    public VirtualFolder(string fileName, IDialogs dialogs, IClipboard clipboard)
    {
        _fileName = fileName;
        _dialogs = dialogs;
        _clipboard = clipboard;
        LastModified = DateTime.Now;
        Name = Path.GetFileName(_fileName);
        Files = new BulkObservableCollection<VirtualFile>();
        SelectedItems = new BulkObservableCollection<VirtualFile>();
        Files.CollectionChanged += OnChange;
        SelectedItems.CollectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RemoveSelectedCommand.NotifyCanExecuteChanged();
        OpenSelectedCommand.NotifyCanExecuteChanged();
        CopySelectedPathCommand.NotifyCanExecuteChanged();
    }

    private void OnChange(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        TotalSize = Files.Sum(f => f.FileSize.TotalBytes);
    }

    public static async Task<VirtualFolder?> LoadFile(string filePath, IDialogs dialogs, IClipboard clipboard)
        => await VirtualFolderFile.LoadFrom(filePath, dialogs, clipboard, CancellationToken.None);

    public async Task Save()
    {
        LastModified = DateTime.Now;
        await VirtualFolderFile.SaveTo(_fileName, this, CancellationToken.None);
    }

    [RelayCommand(CanExecute = nameof(HasSelectedItem))]
    public async Task RemoveSelected()
    {
        if (SelectedItems.Count > 0
            && _dialogs.YesNo(Resources.IdRemoveSelectedItems, Resources.IdConfirm))
        {
            Files.RemoveRange(SelectedItems);
            SelectedItems.Clear();
            await Save();
        }
    }

    [RelayCommand]
    public async Task RemoveDuplicates()
    {
        if (SelectedItems.Count > 0
            && _dialogs.YesNo(Resources.IdRemoveDuplicates, Resources.IdConfirm))
        {
            var duplicates = SelectedItems
                .GroupBy(f => f.FullPath)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

            Files.RemoveRange(duplicates);
            SelectedItems.Clear();
            await Save();
        }
    }


    [RelayCommand(CanExecute = nameof(HasSelectedItem))]
    public void OpenSelected()
    {
        if (SelectedItems.Count > 0)
        {
            foreach (var file in SelectedItems)
            {
                using var process = new Process();
                process.StartInfo.FileName = file.FullPath;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }

        }
    }

    [RelayCommand(CanExecute = nameof(HasSelectedItem))]
    public void CopySelectedPath()
    {
        if (SelectedItems.Count > 0)
        {
            var paths = string.Join(Environment.NewLine, SelectedItems.Select(f => f.FullPath));
            _clipboard.SetText(paths);
        }
    }
}
