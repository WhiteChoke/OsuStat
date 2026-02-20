using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;
using System.ComponentModel;
using System.Windows;

namespace OsuStat.UI.MVVM.ViewModel
{
    public class HomeViewModel : Core.ViewModel
    {
        private readonly ISettingsService _settings;
        public Player User { get; set; } = new();

        public HomeViewModel(ISettingsService settingsService)
        {
            _settings = settingsService;
            _settings.PropertyChanged += OnSettingsChanged;
            LoadData();
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_settings.SetGameFolder))
                LoadData();
        }

        private async void LoadData()
        {
            try
            {
                await LoadUserData.LoadData(User, _settings.CurrentSettings.GameFolder, _settings.ApplicationFolder);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
