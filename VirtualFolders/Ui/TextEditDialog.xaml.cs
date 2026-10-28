using System.Windows;
using System.Windows.Controls;

namespace VirtualFolders.Ui;

/// <summary>
/// Interaction logic for TextEditDialog.xaml
/// </summary>
public partial class TextEditDialog : Window
{
    private readonly int _maxChars;

    public TextEditDialog(int maxChars)
    {
        InitializeComponent();
        _maxChars = maxChars;
    }

    private void OkClick(object sender, RoutedEventArgs e)
        => DialogResult = true;

    private void CancelClick(object sender, RoutedEventArgs e)
        => DialogResult = false;

    private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        => CharCounter?.Text = (_maxChars - TextEditor.Text.Length).ToString();
}
