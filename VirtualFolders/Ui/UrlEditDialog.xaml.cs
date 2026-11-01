using System.Text;
using System.Windows;
using System.Windows.Controls;

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

    private void TbUrl_TextChanged(object sender, TextChangedEventArgs e)
    {
        BtnOk.IsEnabled = Validate(TbUrl.Text, TbName.Text, out string errors);
        TbError.Text = errors;
    }

    private void TbName_TextChanged(object sender, TextChangedEventArgs e)
    {
        BtnOk.IsEnabled = Validate(TbUrl.Text, TbName.Text, out string errors);
        TbError.Text = errors;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        BtnOk.IsEnabled = Validate(TbUrl.Text, TbName.Text, out string errors);
        TbError.Text = errors;
    }

    private static bool Validate(string url, string name, out string errors)
    {
        StringBuilder sb = new();
        if (string.IsNullOrEmpty(url)
            || !Uri.TryCreate(url, UriKind.Absolute, out Uri? parsed)
            || (parsed.Scheme != Uri.UriSchemeHttp && parsed.Scheme != Uri.UriSchemeHttps))
        {
            sb.AppendLine("Invalid URL.");
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            sb.AppendLine("Name cannot be empty.");
        }
        errors = sb.ToString();
        return string.IsNullOrEmpty(errors);
    }
}
