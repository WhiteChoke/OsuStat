using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;
using System.Windows;
using Microsoft.Extensions.Logging;
using OsuStat.Data.Repository;

namespace OsuStat.UI.MVVM.ViewModel
{
    public class HomeViewModel : Core.ViewModel
    {
        private readonly ISettingsService _settings;
        private readonly IDataService _dataService;
        private readonly ILogger<HomeViewModel> _logger;
        public Player User { get; set; } = new();
        public PlayerStat PlayerStat { get; set; }
        public BestScore BestScore { get; set; } 
        private readonly ObservableCollection<BeatMap> _beatMaps;
        public ObservableCollection<BeatMap> BeatMaps { get => _beatMaps; }

        public HomeViewModel
        (
            ISettingsService settingsService,
            PlayerStat playerStat,
            IDataService dataService,
            ObservableCollection<BeatMap> beatMaps,
            ILogger<HomeViewModel> logger,
            BestScore bestScore
            ) 
        {
            PlayerStat = playerStat;
            BestScore = bestScore;
            _logger = logger;
            _settings = settingsService;
            _beatMaps = beatMaps;
            _dataService = dataService;

            _settings.PropertyChanged += async (sender, args) =>
            {
                if (args.PropertyName == nameof(_settings.SetGameFolder))
                {
                        await _dataService.LoadUserInformationAsync(User);
                }
            };
            LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                await _dataService.LoadUserInformationAsync(User);
                await _dataService.LoadStatisticAsync(_beatMaps);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
