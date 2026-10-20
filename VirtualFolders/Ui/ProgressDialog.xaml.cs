using Fluent;
using System.Windows;
using System.Windows.Controls;
using VirtualFolders.Core;

namespace VirtualFolders.Ui;

/// <summary>
/// Interaction logic for ProgressDialog.xaml
/// </summary>
public partial class ProgressDialog : Window, Core.IFileOperationProgress
{
    private readonly CancellationTokenSource _tokenSource;
 
    public ProgressDialog()
    {
        InitializeComponent();
        _tokenSource = new CancellationTokenSource();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
        => Close();

    private bool TryAbort()
    {
        if (Total.Value < Total.Maximum)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to abort the operation?", "Abort", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return (result == MessageBoxResult.Yes);
        }
        else
        {
            return true;
        }
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

    private static string CalculatePercent(ProgressBar progressBar)
    {
        if (progressBar.Maximum == 0)
        {
            return "0%";
        }
        double percent = progressBar.Value / progressBar.Maximum;
        return $"{percent:P2}";
    }

    public CancellationToken CancellationToken
        => _tokenSource.Token;

    public void SetCurrentFileProgress(long fileSize)
    {
        Dispatcher.Invoke(() =>
        {
            Current.Value = fileSize;
            CurrentPercentText.Text = CalculatePercent(Current);
        });
    }

    public void SetCurrentFileSize(long fileSize)
    {
        Dispatcher.Invoke(() =>
        {
            Current.Maximum = fileSize;
            CurrentPercentText.Text = CalculatePercent(Current);
        });
    }

    public void SetTotalFileProgress(long fileSize)
    {
        Dispatcher.Invoke(() =>
        {
            Total.Value = fileSize;
            TotalPercentText.Text = CalculatePercent(Total);
        });

    }

    public void SetTotalSize(long fileSize)
    {
        Dispatcher.Invoke(() =>
        {
            Total.Maximum = fileSize;
            TotalPercentText.Text = CalculatePercent(Total);
        });
    }

    void IFileOperationProgress.ShowDialog()
    {
        Show();
    }
}
