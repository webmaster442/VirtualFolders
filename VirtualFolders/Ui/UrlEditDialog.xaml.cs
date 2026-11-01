using System.Windows;

namespace VirtualFolders.Ui;
/// <summary>
/// Interaction logic for UrlEditDialog.xaml
/// </summary>
public partial class UrlEditDialog : Window
{
    public UrlEditDialog()
    {
        InitializeComponent();
    }

    private void OkClick(object sender, RoutedEventArgs e)
        => DialogResult = true;

    private void CancelClick(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
