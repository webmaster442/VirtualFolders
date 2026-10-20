using ControlzEx.Theming;
using System.Windows;

namespace VirtualFolders;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAccent;
        ThemeManager.Current.SyncTheme();

        base.OnStartup(e);
    }
}

