using Microsoft.Win32;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.Service;

namespace OsuStat.UI.MVVM.ViewModel
{
    public class SettingsViewModel : Core.ViewModel
    {
        private readonly ISettingsService _settings;

        public string CurrentFolder
        {
            get;
            set
            {
                field = "Current folder " + value;
                OnPropertyChanged();
            }
        }
        
        private INavigationService Navigation
        {
            get;
            set 
            { 
                field = value; 
                OnPropertyChanged();
            }
        }

        public RelayCommand SelectFolderCommand { get; set; }
        public RelayCommand NavigateToHomeCommand { get; set; }
        public SettingsViewModel(INavigationService navigationService, ISettingsService settingsService)
        {
            Navigation = navigationService;
            _settings = settingsService;
            CurrentFolder = _settings.CurrentSettings.GameFolder;
            SelectFolderCommand = new RelayCommand(o => { SelectFolder(); });
            NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>(); });
        }

        private void SelectFolder()
        {
            OpenFolderDialog dialog = new()
            {
                Title = "Select osu! folder"
            };
            if (dialog.ShowDialog() != true) return;
            
            _settings.SetGameFolder(dialog.FolderName);
            CurrentFolder = dialog.FolderName;
        }

    }
}
