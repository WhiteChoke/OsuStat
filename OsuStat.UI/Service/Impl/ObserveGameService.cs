using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using OsuStat.Core;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class ObserveGameService : ObservableObject, IObserveGameService
{
    private FileSystemWatcher _watcher;
    private ObservableCollection<BeatMap> _beatmaps;
    private readonly ISettingsService _settings;
    private readonly PlayerStat _playerStat;
    private System.Timers.Timer _searchTimer;
    private System.Timers.Timer _playTimer;
    private Process? _monitoredProcess;
    private const string ProcessName = "osu!";
    private const int AddTimeMs = 60000;

    public ObserveGameService(ISettingsService settings, PlayerStat playerStat)
    {
        _playerStat = playerStat;
        _settings = settings;
    }

    public void Start(ObservableCollection<BeatMap> Beatmaps)
    {
        _beatmaps = Beatmaps;

        BeatMapWatcher();
        _searchTimer = new System.Timers.Timer(2000);
        _searchTimer.Elapsed += (sender, args) => CheckForProcess();
        _searchTimer.Start();

        _playTimer = new System.Timers.Timer(AddTimeMs);
        _playTimer.Elapsed += (sender, args) => _playerStat.PlayTimeMin = 1;
    }


    private void CheckForProcess()
    {
        var process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
        if (process != null && _monitoredProcess == null)
        {
            Console.WriteLine($"{ProcessName} is running");
            _monitoredProcess = process;
            _monitoredProcess.Exited += MonitoredProcessOnExited;
            _monitoredProcess.EnableRaisingEvents = true;
            _playTimer.Start();
        }
    }

    private void MonitoredProcessOnExited(object? sender, EventArgs e)
    {
        Console.WriteLine($"Process {_monitoredProcess.ProcessName} exited");
        _monitoredProcess.Dispose();
        _monitoredProcess = null;
        _searchTimer.Start();
        _playTimer.Stop();
    }

    private void BeatMapWatcher()
    {
        _watcher = new FileSystemWatcher(Path.Combine(_settings.GetGameFolder(), "Data", "r"));
        
        _watcher.NotifyFilter = NotifyFilters.Attributes
                               | NotifyFilters.CreationTime
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.FileName
                               | NotifyFilters.LastAccess
                               | NotifyFilters.LastWrite
                               | NotifyFilters.Security
                               | NotifyFilters.Size;
        
        _watcher.Filter = "*.osr";

        _watcher.Changed += AppendBeatMapEvent;
        _watcher.EnableRaisingEvents = true;
        
        Console.WriteLine($"Beatmap watcher started");
    }

    private async void AppendBeatMapEvent(object sender, FileSystemEventArgs e)
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

        _playerStat.MapPlayed += 1;
        _playerStat.AvgAccuracy = result.accuracy;
        _playerStat.AvgStarRate = result.StarRate;
        _playerStat.AvgBpm = result.Bpm;
        _playerStat.PpGained = result.PpGained;

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