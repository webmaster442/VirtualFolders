using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using VirtualFolders.Core.Format;

namespace VirtualFolders.Core;

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly IDialogs _dialogs;
    private readonly string _settingsFilePath;

    public SettingsViewModel(IDialogs dialogs)
    {
        _dialogs = dialogs;
        _settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "VirtualFolders.json");
        Programs = new BulkObservableCollection<ProgramInformation>();
    }

    public BulkObservableCollection<ProgramInformation> Programs { get; }

    [ObservableProperty]
    public partial ProgramInformation? SelectedProgram { get; set; }

    partial void OnSelectedProgramChanged(ProgramInformation? value)
    {
        RemoveProgramCommand.NotifyCanExecuteChanged();
    }

    private bool HasSelectedProgram()
        => SelectedProgram != null;

    [RelayCommand]
    public async Task AddProgram()
    {
        if (_dialogs.TryOpenFile("Select program", "Executables|*.exe", out string? programPath))
        {
            var programInfo = new ProgramInformation
            {
                DisplayName = Path.GetFileNameWithoutExtension(programPath),
                ProgramPath = programPath
            };
            Programs.Add(programInfo);
        }
        await Save();
    }

    [RelayCommand(CanExecute = nameof(HasSelectedProgram))]
    public async Task RemoveProgram()
    {
        if (SelectedProgram != null)
        {
            Programs.Remove(SelectedProgram);
        }
        await Save();
    }

    public async Task Load()
    {
        if (!File.Exists(_settingsFilePath))
        {
            return;
        }
        ProgramSettings? settings = await ProgramSettingsFile.LoadFrom(_settingsFilePath, CancellationToken.None);
        if (settings != null)
        {
            Programs.Clear();
            Programs.AddRange(settings.Programs.Select(p => new ProgramInformation
            {
                DisplayName = Path.GetFileNameWithoutExtension(p),
                ProgramPath = p
            }));
        }
    }

    private async Task Save()
    {
        ProgramSettings settings = new ProgramSettings
        {
            Programs = Programs.Select(p => p.ProgramPath).ToList()
        };
        await ProgramSettingsFile.SaveTo(_settingsFilePath, settings, CancellationToken.None);
    }
}
