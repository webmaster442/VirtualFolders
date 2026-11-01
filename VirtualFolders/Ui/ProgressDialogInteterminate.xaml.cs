using System.Windows;

using VirtualFolders.Core.Exporters;

namespace VirtualFolders.Ui;
/// <summary>
/// Interaction logic for ProgressDialogInteterminate.xaml
/// </summary>
public partial class ProgressDialogInteterminate : Window, IIndeterminateExportProgress
{
    private readonly CancellationTokenSource _tokenSource;
    private bool _isFinished;

    public CancellationToken CancellationToken
        => _tokenSource.Token;

    public ProgressDialogInteterminate()
    {
        InitializeComponent();
        _tokenSource = new CancellationTokenSource();
        _isFinished = false;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (TryAbort())
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }
        else
        {
            e.Cancel = true;
        }
    }

    private bool TryAbort()
    {
        if (!_isFinished)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to abort the operation?", "Abort", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return (result == MessageBoxResult.Yes);
        }
        else
        {
            return true;
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
        => Close();

    void IIndeterminateExportProgress.ShowDialog()
        => Show();

    public void Finished()
        => _isFinished = true;
}
