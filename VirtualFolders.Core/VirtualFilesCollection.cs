using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace VirtualFolders.Core;

public sealed class VirtualFilesCollection : ObservableCollection<VirtualFile>
{
    public void AddRange(IEnumerable<VirtualFile> files)
    {
        foreach (VirtualFile file in files)
        {
            this.Items.Add(file);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public void RemoveRange(IEnumerable<VirtualFile> files)
    {
        foreach (VirtualFile file in files)
        {
            this.Items.Remove(file);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
