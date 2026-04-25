using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;
using System.Windows;
using Microsoft.Extensions.Logging;
using OsuParsers.Enums;
using OsuStat.Data.Models;
using OsuStat.Data.Repository;

namespace OsuStat.UI.MVVM.ViewModel;

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
        Task.Run(LoadData);
    }

    private void LoadData()
    {
        try
        {
            Task.WhenAll(
                _dataService.LoadUserInformationAsync(User),
                _dataService.LoadStatisticAsync(
                    beatmaps: _beatMaps,
                    stat: PlayerStat,
                    bestScore: BestScore));
            _dataService.DataUpdated +=  DataUpdatedEvent;
            
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void DataUpdatedEvent(object? sender, BeatMap beatmap)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (beatmap.PpGained > BestScore.Pp)
            {
                BestScore.Pp = beatmap.PpGained;
                BestScore.MapName = beatmap.Name;
                BestScore.BgPath = beatmap.BgPath;
            }
            
            if (BeatMaps.Contains(beatmap))
            {
                var oldBeatmap = BeatMaps.First(b => b.Equals(beatmap));
                BeatMaps.Remove(oldBeatmap);
            
                beatmap.PpGained = oldBeatmap.PpGained > beatmap.PpGained
                    ? oldBeatmap.PpGained
                    : beatmap.PpGained;
            
                beatmap.PlayCount = oldBeatmap.PlayCount + 1;
            }
            else
            {
                beatmap.PlayCount = 1;
            }
            
            _beatMaps.Insert(0, beatmap);

        });
        
        _logger.Log(LogLevel.Information, "View successfully updated");
    }
}

