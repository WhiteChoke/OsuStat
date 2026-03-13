using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;
using System.ComponentModel;
using System.Windows;

namespace OsuStat.UI.MVVM.ViewModel
{
    public class HomeViewModel : Core.ViewModel
    {
        private readonly ISettingsService _settings;
        private readonly IObserveGameService _observeGameService;
        public Player User { get; set; } = new();
        public PlayerStat PlayerStat { get; set; }


        private readonly ObservableCollection<BeatMap> _beatMaps;
        public ObservableCollection<BeatMap> BeatMaps { get => _beatMaps; }

        public HomeViewModel
        (
            ISettingsService settingsService,
            IObserveGameService observeGameService,
            PlayerStat playerStat
            ) 
        {
            PlayerStat = playerStat;
            _settings = settingsService;
            _beatMaps = new ObservableCollection<BeatMap>();
            _observeGameService = observeGameService;
            _settings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_settings.SetGameFolder))
                {
                    LoadData();
                }
            };
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                await LoadUserData.LoadData(User, _settings.GetGameFolder(), _settings.ApplicationFolder);
                _observeGameService.Start(_beatMaps);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
