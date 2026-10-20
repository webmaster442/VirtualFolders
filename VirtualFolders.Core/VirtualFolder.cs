using CommunityToolkit.Mvvm.ComponentModel;
using VirtualFolders.Core.Format;

namespace VirtualFolders.Core;

public sealed partial class VirtualFolder : ObservableObject
{
    private readonly string _fileName;

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial DateTime LastModified { get; set; }

    [ObservableProperty]
    public partial VirtualFile? SelectedFile { get; set; }

    [ObservableProperty]
    public partial FileSize TotalSize { get; private set; }

    public VirtualFilesCollection Files { get; }

    public VirtualFolder(string fileName)
    {
        _fileName = fileName;
        LastModified = DateTime.Now;
        Name = Path.GetFileName(_fileName);
        Files = new VirtualFilesCollection();
        Files.CollectionChanged += OnChange;
    }

    private void OnChange(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        TotalSize = Files.Sum(f => f.FileSize.TotalBytes);
    }

    public static async Task<VirtualFolder?> LoadFile(string filePath)
        => await VirtualFolderFile.LoadFrom(filePath, CancellationToken.None);

    public async Task Save()
    {
        LastModified = DateTime.Now;
        await VirtualFolderFile.SaveTo(_fileName, this, CancellationToken.None);
    }
}
