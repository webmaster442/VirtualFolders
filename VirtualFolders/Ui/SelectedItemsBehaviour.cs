using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

using VirtualFolders.Core;

namespace VirtualFolders.Ui;

internal sealed class SelectedItemsBehaviour : Behavior<DataGrid>
{
    public static readonly DependencyProperty SelectedItemsProperty
        = DependencyProperty.Register(nameof(SelectedItems),
            typeof(BulkObservableCollection<VirtualFile>),
            typeof(SelectedItemsBehaviour),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


    public BulkObservableCollection<VirtualFile> SelectedItems
    {
        get => (BulkObservableCollection<VirtualFile>)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += OnSelectionChanged;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.SelectionChanged -= OnSelectionChanged;
        base.OnDetaching();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedItems == null)
            return;

        var files = e.AddedItems.Cast<VirtualFile>();
        var removedFiles = e.RemovedItems.Cast<VirtualFile>();
        SelectedItems.RemoveRange(removedFiles);
        SelectedItems.AddRange(files);
    }
}