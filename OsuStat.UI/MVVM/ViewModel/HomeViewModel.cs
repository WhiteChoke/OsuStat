using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;
using System.ComponentModel;
using System.Windows;
using OsuStat.UI.Service.Impl;

namespace OsuStat.UI.MVVM.ViewModel
{
    public class HomeViewModel : Core.ViewModel
    {
        private readonly ISettingsService _settings;
        private readonly IDataService _dataService;
        public Player User { get; set; } = new();
        public PlayerStat PlayerStat { get; set; }


        private readonly ObservableCollection<BeatMap> _beatMaps;
        public ObservableCollection<BeatMap> BeatMaps { get => _beatMaps; }

        public HomeViewModel
        (
            ISettingsService settingsService,
            PlayerStat playerStat,
            IDataService dataService,
            ObservableCollection<BeatMap> beatMaps
            ) 
        {
            PlayerStat = playerStat;
            _settings = settingsService;
            _beatMaps = beatMaps;
            _dataService = dataService;
            _settings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_settings.SetGameFolder))
                {
                    Task.Run(async () => await LoadUserData.LoadData(User, _settings.GameFolder, _settings.ApplicationFolder));
                }
            };
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                await LoadUserData.LoadData(User, _settings.GameFolder, _settings.ApplicationFolder);
                _dataService.UploadData(_beatMaps);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
