using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using OsuStat.Core;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class ObserveGameService : ObservableObject, IObserveGameService
{
    private FileSystemWatcher watcher;
    private ObservableCollection<BeatMap> _beatmaps;
    private ISettingsService _settings;
    private PlayerStat _playerStat;

    public ObserveGameService(ISettingsService settings, PlayerStat playerStat)
    {
        _playerStat = playerStat;
        _settings = settings;
    }
    
    public void Start(ObservableCollection<BeatMap> Beatmaps)
    {
        watcher = new FileSystemWatcher(Path.Combine(_settings.GetGameFolder(), "Data", "r"));
        _beatmaps = Beatmaps;
        
        watcher.NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size;
        
        watcher.Filter = "*.osr";

        watcher.Changed += AppendBeatMap;
        watcher.EnableRaisingEvents = true;
    }

    private async void AppendBeatMap(object sender, FileSystemEventArgs e)
    {
        
        var result = await ReplayInfo.Get(e.FullPath, _settings.GetGameFolder());

        if (result == null)
        {
            MessageBox.Show("Failed to load beatmap information", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        var bg = result.BgPath.Length != 0
            ? result.BgPath
            : Path.Combine(_settings.ApplicationFolder, "Assets", "Images", "Bg fuck up.jpg");
        
        var beatmap = new BeatMap(
            result.Name,
            result.Artist,
            result.Mapper,
            0,
            result.Bpm,
            result.Length,
            result.StarRate,
            result.Hp,
            result.Cs,
            result.Ar,
            bg,
            result.PpGained,
            result.MaxCombo,
            DateTime.Now.ToLongDateString()
        );

        var writeCount = _playerStat.WriteCount;
        _playerStat.AvgAccuracy = 98;
        _playerStat.AvgStarRate = result.StarRate.CompareTo(2);
        _playerStat.AvgBpm = result.Bpm;
        _playerStat.PpGained = result.PpGained;
        _playerStat.MapPlayed = writeCount;

        if (!_beatmaps.Any(map => map.Equals(beatmap)))
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _beatmaps.Add(beatmap);
            });
        }
        
        Console.WriteLine($"Triggered beatmap append {e.ChangeType}" );
    }
    
    
}