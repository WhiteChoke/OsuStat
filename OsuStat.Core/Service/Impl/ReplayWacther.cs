using Microsoft.Extensions.Logging;
using OsuStat.Core.Extractor;
using OsuStat.Core.Model;
using OsuStat.Core.Service.Interfaces;

namespace OsuStat.Core.Service.Impl;

public class ReplayWatcher : IReplayWatcher
{
    private string _gameFolder;
    private FileSystemWatcher _watcher;
    private readonly ILogger<ReplayWatcher> _logger;
    private readonly PlayedBeatmap _playedBeatmap;  
    public event EventHandler<ReplayData>? OnReplayRegistered;

    public ReplayWatcher(
        ILogger<ReplayWatcher> logger, 
        PlayedBeatmap playedBeatmap)
    {
        _logger = logger;
        _playedBeatmap = playedBeatmap;
    }

    public void Start(string gameDirectory)
    { 
        _gameFolder = gameDirectory;
        
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
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _logger.LogInformation("Beatmap watcher stopped");
    }

    private async void ReplayRegistered(object sender, FileSystemEventArgs e)
    {
        var beatmapDirectoryPath = Path.Combine(_gameFolder, "Songs", _playedBeatmap.MapDirectory);
        var result = await ReplayExtractor.Extract(
            e.FullPath, 
            [beatmapDirectoryPath, _playedBeatmap.OsuFileName]
            );
        OnReplayRegistered?.Invoke(this, result);
        _logger.LogInformation("Registered a play of beatmap {name}", result.Name);
    }
}