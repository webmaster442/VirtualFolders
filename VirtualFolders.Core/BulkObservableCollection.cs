using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace VirtualFolders.Core;

public sealed class BulkObservableCollection<T> : ObservableCollection<T>
{
    public void AddRange(IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            this.Items.Add(item);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public void RemoveRange(IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            this.Items.Remove(item);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
