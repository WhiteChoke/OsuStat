using Microsoft.Extensions.Logging;
using OsuStat.Core.Extractor;
using OsuStat.Core.Model;
using OsuStat.Core.Service.Interfaces;

namespace OsuStat.Core.Service.Impl;

public class ReplayWatcher : IReplayWatcher
{
    private FileSystemWatcher? _watcher;
    private readonly ILogger<ReplayWatcher> _logger;
    private readonly string _gameFolder = @"D:\osu!";
    public event EventHandler<ReplayData>? OnReplayRegistered;

    public ReplayWatcher(
        ILogger<ReplayWatcher> logger)
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
        OnReplayRegistered?.Invoke(this, result);
        _logger.LogInformation("Beatmap added: {name}", result.Name);
    }
}