using System.Windows;
using System.Windows.Media;

namespace VirtualFolders.Ui;

internal static class Extensions
{
    public static T? FindParentControl<T>(this DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);
        if (parent == null)
        {
            return null;
        }
        
        if (parent is T t)
        {
            return t;
        }

        return FindParentControl<T>(parent);
    }
}