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

    public ObserveGameService(ISettingsService settings)
    {
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

        var beatmap = new BeatMap(
            (string) result["Name"],
            (string) result["Artist"],
            (string) result["Mapper"],
            0,
            (double) result["Bpm"],
            (int) result["Length"],
            (double) result["StarRate"],
            (double) result["Hp"],
            (double) result["Cs"],
            (double) result["Ar"],
            (string) result["BgPath"]
        );



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