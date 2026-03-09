using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using OsuParsers.Beatmaps;
using OsuStat.Core;
using OsuStat.UI.Dto;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class ObserveGameService(ISettingsService settings) : ObservableObject, IObserveGameService
{
    private FileSystemWatcher watcher;
    private ObservableCollection<BeatMap> _beatmaps;
    private HttpClient  _httpClient = new();
    
    public void Start(ObservableCollection<BeatMap> Beatmaps)
    {
        watcher = new FileSystemWatcher(Path.Combine(@"D:\osu!\Data\r"));
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
        watcher.Created += AppendBeatMap;
        
        watcher.EnableRaisingEvents = true;
    }

    private async void AppendBeatMap(object sender, FileSystemEventArgs e)
    {
        
        var result = await ReplayInfo.Get(e.FullPath, @"D:/osu!");

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
    }
    
    
}