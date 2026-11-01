namespace VirtualFolders.Core;

internal sealed class SelectionChangedMessage
{
    public List<VirtualFile> SelectedItems { get; }

    public SelectionChangedMessage(IEnumerable<VirtualFile> selectedItems)
    {
        SelectedItems = selectedItems.ToList();
    }
}
