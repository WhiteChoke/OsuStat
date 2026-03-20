using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Logging;
using OsuParsers.Enums;
using OsuStat.Core;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class ReplayWatcher : ObservableObject, IReplayWatcher
{
    private FileSystemWatcher _watcher;
    private readonly ObservableCollection<BeatMap> _beatmaps;
    private readonly ISettingsService _settings;
    private readonly PlayerStat _playerStat;
    private readonly ILogger<ReplayWatcher> _logger;
    private readonly IDataService _dataService;

    public ReplayWatcher(ObservableCollection<BeatMap> beatmaps, ISettingsService settings, PlayerStat playerStat, ILogger<ReplayWatcher> logger, IDataService dataService)
    {
        _beatmaps = beatmaps;
        _settings = settings;
        _playerStat = playerStat;
        _logger = logger;
        _dataService = dataService;
    }

    public void Start()
    { 
        _watcher = new FileSystemWatcher(Path.Combine(_settings.GameFolder, "Data", "r"));
        
        _watcher.NotifyFilter = NotifyFilters.CreationTime
                                | NotifyFilters.LastWrite;
        
        _watcher.Filter = "*.osr";

        _watcher.Changed += AppendBeatmapEvent;
        _watcher.EnableRaisingEvents = true;
        
        _logger.LogInformation("Beatmap watcher started");
    }

    public void Stop()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        
        _logger.LogInformation("Beatmap watcher stopped");
    }

    private async void AppendBeatmapEvent(object sender, FileSystemEventArgs e)
    {
        try
        {
            var result = await ReplayInfo.Get(e.FullPath, _settings.GameFolder);

            if (result == null)
            {
                MessageBox.Show("Failed to load beatmap information", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var bg = result.BgPath ?? Path.Combine(_settings.ApplicationFolder, "Assets", "Images", "Bg fuck up.jpg");

            var modIcons = GetIconPathList(result.Mods);
            
            var beatmap = new BeatMap(
                result.Name,
                result.Artist,
                result.Mapper,
                result.Bpm,
                result.Length,
                result.StarRate,
                result.Hp,
                result.Cs,
                result.Ar,
                bg,
                result.PpGained,
                result.MaxCombo,
                result.Accuracy,
                modIcons,
                result.Grade
            );

            await Application.Current.Dispatcher.InvokeAsync(() =>
                UpdateData(beatmap, result.Accuracy)
                );

            await _dataService.SaveDataAsync(_playerStat, _settings.SavePlayerStatDirectoryPath);
            await _dataService.SaveDataAsync(_beatmaps.ToList(), _settings.SaveScoreDirectoryPath);

            _logger.LogInformation("Beatmap added: {name}", beatmap.Name);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateData(BeatMap beatmap, double accuracy) {
        _playerStat.Update(beatmap.Bpm, beatmap.PpGained, beatmap.StarRate, accuracy);

        if (_beatmaps.Contains(beatmap))
        {
            var oldBeatmap = _beatmaps.First(b => b.Equals(beatmap));
            _beatmaps.Remove(oldBeatmap);
            
            beatmap.PpGained = oldBeatmap.PpGained > beatmap.PpGained
                ? oldBeatmap.PpGained
                : beatmap.PpGained;
            
            beatmap.PlayCount = oldBeatmap.PlayCount + 1;
        }
        else
        {
            beatmap.PlayCount = 1;
        }
        
        _beatmaps.Insert(0, beatmap);
    }

    private List<string> GetIconPathList(List<Mods> mods)
    {
        return 
            mods.Select(mod => 
                Path.Combine(_settings.ModIconsFolder, $"{mod}.png"))
                .ToList();
    }
}