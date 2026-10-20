using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VirtualFolders.Core.Properties;

namespace VirtualFolders.Core;

public sealed partial class VirtualFolders : ObservableObject
{
    private readonly IDialogs _dialogs;
    private readonly string _filterstring;
    private readonly string _virtualFolderFilterString;

    public VirtualFolders(IDialogs dialogs)
    {
        _dialogs = dialogs;
        _filterstring = new FilterstringBuilder().WithAllFiles().Build();
        _virtualFolderFilterString = new FilterstringBuilder().WithVirtualFolders().Build();
        Folders = new ObservableCollection<VirtualFolder>();
        ActualFolder = null;
    }

    public ObservableCollection<VirtualFolder> Folders { get; }

    [ObservableProperty]
    public partial VirtualFolder? ActualFolder { get; set; }

    partial void OnActualFolderChanged(VirtualFolder? value)
    {
        CloseActualFolderCommand.NotifyCanExecuteChanged();
        AddFilesCommand.NotifyCanExecuteChanged();
        AddFolderCommand.NotifyCanExecuteChanged();
        RemoveSelectedCommand.NotifyCanExecuteChanged();
        RemoveAllCommand.NotifyCanExecuteChanged();
        RemoveNotExistingCommand.NotifyCanExecuteChanged();
        ExportToFolderCommand.NotifyCanExecuteChanged();
        ExportZipCommand.NotifyCanExecuteChanged();
        ExportM3uCommand.NotifyCanExecuteChanged();
        ExportPlsCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public async Task New()
    {
        if (_dialogs.TrySaveFile(Resources.IdNewVirtualFolder, _virtualFolderFilterString, out string? filePath))
        {
            try
            {
                var newFolder = new VirtualFolder(filePath);
                Folders.Add(newFolder);
                ActualFolder = newFolder;
                await newFolder.Save();
            }
            catch (Exception ex)
            {
                _dialogs.Error(Resources.IdFolderCreateFailed.Format(ex), Resources.IdError);
            }
        }
    }

    [RelayCommand]
    public async Task Open()
    {
        if (_dialogs.TryOpenFile(Resources.IdOpenVirtualFolder, _virtualFolderFilterString, out string? filePath))
        {
            try
            {
                VirtualFolder? folder = await VirtualFolder.LoadFile(filePath);
                if (folder == null)
                {
                    _dialogs.Error(Resources.IdFileLoadFailed, Resources.IdError);
                    return;
                }
                Folders.Add(folder);
                ActualFolder = folder;
            }
            catch (Exception ex)
            {
                _dialogs.Error(Resources.IdFolderOpenFailed.Format(ex), Resources.IdError);
            }
        }
    }

    [RelayCommand]
    public void CloseActualFolder()
    {
        if (ActualFolder != null)
        {
            Folders.Remove(ActualFolder);
            int newActual = Folders.Count - 1;
            ActualFolder = newActual >= 0 ? Folders[newActual] : null;
        }
    }

    [RelayCommand]
    public void CloseAll()
    {
        Folders.Clear();
        ActualFolder = null;
    }

    public bool HasOpenFolder()
    {
        return ActualFolder is not null;
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolder))]
    public async Task AddFiles()
    {
        if (ActualFolder is not null &&
            _dialogs.TryOpenFiles(Resources.IdAddFiles, _filterstring, out IEnumerable<string>? files))
        {
            ActualFolder.Files.AddRange(files.Select(x => VirtualFile.CreateFromFilePath(x)));
            await ActualFolder.Save();
        }
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolder))]
    public async Task AddFolder()
    {
        if (ActualFolder is not null &&
            _dialogs.TryOpenFolder(Resources.IdAddFolder, out string? folder))
        {
            var files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
            ActualFolder.Files.AddRange(files.Select(x => VirtualFile.CreateFromFilePath(x)));
            await ActualFolder.Save();
        }
    }

    private bool CanRemoveSelected()
    {
        return ActualFolder is not null
            && ActualFolder.SelectedFile is not null;
    }

    [RelayCommand]
    public async Task FilesDropped(string[] files)
    {
        if (ActualFolder is not null
            && files.Length > 0)
        {
            ActualFolder.Files.AddRange(files.Select(x => VirtualFile.CreateFromFilePath(x)));
            await ActualFolder.Save();
        }
    }

    [RelayCommand(CanExecute = nameof(CanRemoveSelected))]
    public async Task RemoveSelected()
    {
        if (ActualFolder is not null
            && ActualFolder.SelectedFile is not null
            && _dialogs.YesNo(Resources.IdRemoveSelectedFile, Resources.IdConfirm))
        {
            ActualFolder.Files.Remove(ActualFolder.SelectedFile);
            ActualFolder.SelectedFile = null;
            await ActualFolder.Save();
        }
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolder))]
    public async Task RemoveNotExisting()
    {
        if (ActualFolder is not null
            && _dialogs.YesNo(Resources.IdRemoveNotExistingFiles, Resources.IdConfirm))
        {
            var notExistingFiles = ActualFolder.Files.Where(x => !File.Exists(x.FullPath)).ToList();
            ActualFolder.Files.RemoveRange(notExistingFiles);
            await ActualFolder.Save();
        }
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolder))]
    public async Task RemoveAll()
    {
        if (ActualFolder is not null
            && _dialogs.YesNo(Resources.IdRemoveAllFiles, Resources.IdConfirm))
        {
            ActualFolder.Files.Clear();
            await ActualFolder.Save();
        }
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolder))]
    public async Task ExportZip()
    {
        if (ActualFolder is not null
            && _dialogs.TrySaveFile(Resources.IdExportZip, "Zip Files (*.zip)|*.zip", out string? targetFile))
        {
            IFileOperationProgress progress = _dialogs.ShowFileOperationProgress(Resources.IdCompressingFiles);
            await FileOperations.CreateZipFromVirtualFolder(ActualFolder, targetFile, progress);
        }
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolder))]
    public async Task ExportToFolder()
    {
        if (ActualFolder is not null
            && _dialogs.TryOpenFolder(Resources.IdExportFolder, out string? targetFolder))
        {
            IFileOperationProgress progress = _dialogs.ShowFileOperationProgress(Resources.IdCopyFiles);
            await FileOperations.CopyVirtualFolderToDirectory(ActualFolder, targetFolder, progress);
        }
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolder))]
    public async Task ExportM3u()
    {
        if (ActualFolder is not null
            && _dialogs.TrySaveFile(Resources.IdExportPlaylist, "M3U Files (*.m3u)|*.m3u", out string? targetFile))
        {
            IFileOperationProgress progress = _dialogs.ShowFileOperationProgress(Resources.IdExportPlaylist);
            await FileOperations.ExportM3U(ActualFolder, targetFile, progress);
        }
    }


    [RelayCommand(CanExecute = nameof(HasOpenFolder))]
    public async Task ExportPls()
    {
        if (ActualFolder is not null
            && _dialogs.TrySaveFile(Resources.IdExportPlaylist, "PLS Files (*.pls)|*.pls", out string? targetFile))
        {
            IFileOperationProgress progress = _dialogs.ShowFileOperationProgress(Resources.IdExportPlaylist);
            await FileOperations.ExportPls(ActualFolder, targetFile, progress);
        }
    }
}
