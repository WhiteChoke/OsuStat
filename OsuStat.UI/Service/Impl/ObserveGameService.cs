using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using OsuStat.Core;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class ObserveGameService(ISettingsService settings) : ObservableObject, IObserveGameService
{
    private FileSystemWatcher watcher;
    private ObservableCollection<BeatMap> _beatmaps;
    
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

    private void AppendBeatMap(object sender, FileSystemEventArgs e)
    {
        var result = ReplayInfo.Get(e.FullPath, @"D:\osu!");
        var hp = double.Parse(result["Hp"]);
        var сs = double.Parse(result["Cs"]);
        var ar = double.Parse(result["Ar"]);
        var lenght = int.Parse(result["Length"]);
        var beatmap = new BeatMap(
            result["Name"],
            result["Artist"],
            result["Mapper"],
            1,
            1,
            lenght,
            0.0,
            hp,
            сs,
            ar,
            result["BgPath"]
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