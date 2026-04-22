using Microsoft.Extensions.Logging;
using OsuStat.Core.Extractor;
using OsuStat.Core.Service.Interfaces;

namespace OsuStat.Core.Service.Impl;

public class ReplayWatcher : IReplayWatcher
{
    private FileSystemWatcher? _watcher;
    private readonly ILogger<ReplayWatcher> _logger;
    private string _gameFolder = @"D:\osu!";
    public event EventHandler? OnReplayRegistered;

    public ReplayWatcher(ILogger<ReplayWatcher> logger)
    {
        _logger = logger;
    }

    public void Start()
    { 
        _watcher = new FileSystemWatcher(Path.Combine(_gameFolder, "Data", "r"));
        
        _watcher.NotifyFilter = NotifyFilters.CreationTime
                                | NotifyFilters.LastWrite;
        
        _watcher.Filter = "*.osr";

        _watcher.Changed += ReplayRegistered;
        _watcher.EnableRaisingEvents = true;
        
        _logger.LogInformation("Beatmap watcher started");
    }

    public void Stop()
    {
        _watcher?.EnableRaisingEvents = false;
        _watcher?.Dispose();
        
        _logger.LogInformation("Beatmap watcher stopped");
    }

    private void ReplayRegistered(object sender, FileSystemEventArgs e)
    {
        var result = ReplayExtractor.Extract(e.FullPath, _gameFolder);
        OnReplayRegistered?.Invoke(sender, e);
        _logger.LogInformation("Beatmap added: {name}", result.Name);
    }

    // private void UpdateData(BeatMap beatmap, double accuracy) {
    //     _playerStat.Update(beatmap.Bpm, beatmap.PpGained, beatmap.StarRate, accuracy);
    //
    //     if (beatmap.PpGained > _bestScore.Pp)
    //     {
    //         _bestScore.Pp = beatmap.PpGained;
    //         _bestScore.MapName = beatmap.Name;
    //         _bestScore.BgPath = beatmap.BgPath;
    //     }
    //     
    //     if (_beatmaps.Contains(beatmap))
    //     {
    //         var oldBeatmap = _beatmaps.First(b => b.Equals(beatmap));
    //         _beatmaps.Remove(oldBeatmap);
    //         
    //         beatmap.PpGained = oldBeatmap.PpGained > beatmap.PpGained
    //             ? oldBeatmap.PpGained
    //             : beatmap.PpGained;
    //         
    //         beatmap.PlayCount = oldBeatmap.PlayCount + 1;
    //     }
    //     else
    //     {
    //         beatmap.PlayCount = 1;
    //     }
    //     
    //     _beatmaps.Insert(0, beatmap);
    // }
    //
    // private List<string> GetIconPathList(List<Mods> mods)
    // {
    //     return 
    //         mods.Select(mod => 
    //             Path.Combine(_settings.ModIconsFolder, $"{mod}.png"))
    //             .ToList();
    // }
}