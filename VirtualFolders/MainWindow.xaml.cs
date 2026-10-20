using System.Windows;

namespace VirtualFolders;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new Core.VirtualFolders(new Dialogs());
    }

    private void DecideDragDrop(DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop)
            && DataContext is Core.VirtualFolders vm
            && vm.HasOpenFolder())
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void RibbonWindow_DragEnter(object sender, System.Windows.DragEventArgs e)
    => DecideDragDrop(e);

    private void RibbonWindow_DragOver(object sender, System.Windows.DragEventArgs e)
        => DecideDragDrop(e);

    private async void RibbonWindow_Drop(object sender, System.Windows.DragEventArgs e)
    {
        if (DataContext is Core.VirtualFolders vm
            && e.Data.GetData(DataFormats.FileDrop) is string[] files)
        {
            await vm.FilesDropped(files);
        }
    }
}
