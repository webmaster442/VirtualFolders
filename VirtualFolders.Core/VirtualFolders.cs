using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using VirtualFolders.Core.Exporters;
using VirtualFolders.Core.Properties;

namespace VirtualFolders.Core;

public sealed partial class VirtualFolders : ObservableObject, IRecipient<SelectionChangedMessage>
{
    private readonly IDialogs _dialogs;
    private readonly IClipboard _clipboard;
    private readonly string _filterstring;
    private readonly string _virtualFolderFilterString;

    public VirtualFolders(IDialogs dialogs, IClipboard clipboard)
    {
        _dialogs = dialogs;
        _clipboard = clipboard;
        _filterstring = new FilterstringBuilder().WithAllFiles().Build();
        _virtualFolderFilterString = new FilterstringBuilder().WithVirtualFolders().Build();
        Folders = new ObservableCollection<VirtualFolder>();
        ActualFolder = null;
        Settings = new SettingsViewModel(dialogs);
        WeakReferenceMessenger.Default.Register<SelectionChangedMessage>(this);
    }

    public ObservableCollection<VirtualFolder> Folders { get; }

    public SettingsViewModel Settings { get; }

    [ObservableProperty]
    public partial VirtualFolder? ActualFolder { get; set; }

    partial void OnActualFolderChanged(VirtualFolder? value)
    {
        OnPropertyChanged(nameof(HasOpenFolder));
        CloseActualFolderCommand.NotifyCanExecuteChanged();
        AddFilesCommand.NotifyCanExecuteChanged();
        AddFolderCommand.NotifyCanExecuteChanged();
        RemoveAllCommand.NotifyCanExecuteChanged();
        RemoveNotExistingCommand.NotifyCanExecuteChanged();
        ExportZipCommand.NotifyCanExecuteChanged();
        ExportPlsCommand.NotifyCanExecuteChanged();
        ExportFolderCommand.NotifyCanExecuteChanged();
        ExportM3uCommand.NotifyCanExecuteChanged();
        ExportIsoCommand.NotifyCanExecuteChanged();
        AddUrlCommand.NotifyCanExecuteChanged();
    }

    void IRecipient<SelectionChangedMessage>.Receive(SelectionChangedMessage message)
    {
        ExportZipCommand.NotifyCanExecuteChanged();
        ExportPlsCommand.NotifyCanExecuteChanged();
        ExportFolderCommand.NotifyCanExecuteChanged();
        ExportM3uCommand.NotifyCanExecuteChanged();
        ExportIsoCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    public async Task New()
    {
        if (_dialogs.TrySaveFile(Resources.IdNewVirtualFolder, _virtualFolderFilterString, out string? filePath))
        {
            try
            {
                var newFolder = new VirtualFolder(filePath, _dialogs, _clipboard);
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
                VirtualFolder? folder = await VirtualFolder.LoadFile(filePath, _dialogs, _clipboard);
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

    public bool HasOpenFolder
        => ActualFolder is not null;

    public bool HasOpenFolderCheck() => HasOpenFolder;

    public bool ExportCheck(IReadOnlyList<VirtualFile> files)
        => HasOpenFolder && files?.Count > 0;

    [RelayCommand(CanExecute = nameof(HasOpenFolderCheck))]
    public async Task AddFiles()
    {
        if (ActualFolder is not null &&
            _dialogs.TryOpenFiles(Resources.IdAddFiles, _filterstring, out IEnumerable<string>? files))
        {
            ActualFolder.Files.AddRange(files.Select(x => VirtualFile.CreateFromFilePath(x)));
            await ActualFolder.Save();
        }
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolderCheck))]
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

    [RelayCommand(CanExecute = nameof(HasOpenFolderCheck))]
    public async Task AddUrl()
    {
        if (ActualFolder is not  null
            && _dialogs.TryAddUrl(Resources.IdAddUrl, out string? url))
        {
            ActualFolder.Files.Add(VirtualFile.CreateFromUrl(url));
            await ActualFolder.Save();
        }
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

    [RelayCommand(CanExecute = nameof(HasOpenFolderCheck))]
    public async Task RemoveNotExisting()
    {
        if (ActualFolder is not null
            && _dialogs.YesNo(Resources.IdRemoveNotExistingItems, Resources.IdConfirm))
        {
            var notExistingFiles = ActualFolder.Files.Where(x => !File.Exists(x.FullPath) && !x.IsUrl).ToList();
            ActualFolder.Files.RemoveRange(notExistingFiles);
            await ActualFolder.Save();
        }
    }

    [RelayCommand(CanExecute = nameof(HasOpenFolderCheck))]
    public async Task RemoveAll()
    {
        if (ActualFolder is not null
            && _dialogs.YesNo(Resources.IdRemoveAllItems, Resources.IdConfirm))
        {
            ActualFolder.Files.Clear();
            await ActualFolder.Save();
        }
    }

    [RelayCommand(CanExecute = nameof(ExportCheck))]
    public async Task ExportFolder(IReadOnlyList<VirtualFile> files)
    {
        if (ActualFolder is not null 
            && files?.Count > 0
            && _dialogs.TryOpenFolder(Resources.IdExportFolder, out string? targetFolder))
        {
            IExportProgress progress = _dialogs.CreateFileOperationProgress(Resources.IdCopyFiles);
            var exporter = new Exporters.DirectoryExporter();
            await exporter.Execute(files, targetFolder, progress);
        }
    }

    [RelayCommand(CanExecute = nameof(ExportCheck))]
    public async Task ExportZip(IReadOnlyList<VirtualFile> files)
    {
        if (ActualFolder is not null
            && files?.Count > 0
            && _dialogs.TrySaveFile(Resources.IdExportZip, "Zip Files (*.zip)|*.zip", out string? targetFile))
        {
            IExportProgress progress = _dialogs.CreateFileOperationProgress(Resources.IdCompressingFiles);
            var exporter = new Exporters.ZipExporter();
            await exporter.Execute(files, targetFile, progress);
        }
    }

    [RelayCommand(CanExecute = nameof(ExportCheck))]
    public async Task ExportM3u(IReadOnlyList<VirtualFile> files)
    {
        if (ActualFolder is not null
            && files?.Count > 0
            && _dialogs.TrySaveFile(Resources.IdExportPlaylist, "M3U Files (*.m3u)|*.m3u", out string? targetFile))
        {
            IExportProgress progress = _dialogs.CreateFileOperationProgress(Resources.IdExportPlaylist);
            var exporter = new Exporters.M3UExporter();
            if (!exporter.IsWithinSizeLimit(files, _dialogs))
            {
                return;
            }
            await exporter.Execute(files, targetFile, progress);
        }
    }

    [RelayCommand(CanExecute = nameof(ExportCheck))]
    public async Task ExportPls(IReadOnlyList<VirtualFile> files)
    {
        if (ActualFolder is not null
            && files?.Count > 0
            && _dialogs.TrySaveFile(Resources.IdExportPlaylist, "PLS Files (*.pls)|*.pls", out string? targetFile))
        {
            IExportProgress progress = _dialogs.CreateFileOperationProgress(Resources.IdExportPlaylist);
            var exporter = new Exporters.PlsExporter();
            if (!exporter.IsWithinSizeLimit(files, _dialogs))
            {
                return;
            }
            await exporter.Execute(files, targetFile, progress);
        }
    }

    [RelayCommand(CanExecute = nameof(ExportCheck))]
    public async Task ExportTar(IReadOnlyList<VirtualFile> files)
    {
        if (ActualFolder is not null
            && files?.Count > 0
            && _dialogs.TrySaveFile(Resources.IdExportTar, "TAR Files (*.tar)|*.tar", out string? targetFile))
        {
            IIndeterminateExportProgress progress = _dialogs.CreateInteterminateFileOperationProgress(Resources.IdProcessingFiles);
            var exporter = new Exporters.TarExporter();
            if (!exporter.IsWithinSizeLimit(files, _dialogs))
            {
                return;
            }
            await exporter.Execute(files, targetFile, progress);
        }
    }

    [RelayCommand(CanExecute = nameof(ExportCheck))]
    public async Task ExportIso(IReadOnlyList<VirtualFile> files)
    {
        if (ActualFolder is not null
            && files?.Count > 0
            && _dialogs.TryEditText(Resources.IdIsoVolumeLabel, "", 256, out string? volumeLabel)
            && _dialogs.TrySaveFile(Resources.IdExportIso, "ISO Files (*.iso)|*.iso", out string? targetFile))
        {
            IIndeterminateExportProgress progress = _dialogs.CreateInteterminateFileOperationProgress(Resources.IdProcessingFiles);
            var exporter = new Exporters.IsoExporter(volumeLabel);
            if (!exporter.IsWithinSizeLimit(files, _dialogs))
            {
                return;
            }
            await exporter.Execute(files, targetFile, progress);
        }
    }
}
