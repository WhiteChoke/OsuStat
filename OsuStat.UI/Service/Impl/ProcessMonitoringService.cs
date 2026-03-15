using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class ProcessMonitoringService : IProcessMonitoringService
{
    private FileSystemWatcher _watcher;
    private readonly ISettingsService _settings;
    private readonly PlayerStat _playerStat;
    private readonly System.Timers.Timer _searchProcessTimer = new(2000);
    private readonly System.Timers.Timer _playTimer =  new(60000);
    private Process? _monitoredProcess;
    private readonly ILogger<ProcessMonitoringService> _logger;
    private readonly IDataService _dataService;
    private readonly IReplayWatcher _replayWatcher;
    private const string ProcessName = "osu!";
    
    public ProcessMonitoringService(ISettingsService settings, PlayerStat playerStat, IDataService dataService, ILogger<ProcessMonitoringService> logger, IReplayWatcher replayWatcher)
    {
        _logger = logger;
        _playerStat = playerStat;
        _settings = settings;
        _dataService = dataService;
        _replayWatcher = replayWatcher;
    }
    
    public void Run()
    {
        _searchProcessTimer.Elapsed += (sender, args) => CheckForProcess();
        _searchProcessTimer.Start();

        _playTimer.Elapsed += async (sender, args) =>
        {
            _playerStat.PlayTimeMin = 1;
            await _dataService.SaveDataAsync(_playerStat, _settings.SavePlayerStatDirectoryPath);
        };
    }
    
    private void CheckForProcess()
    {
        var process = Process.GetProcessesByName(ProcessName).FirstOrDefault();
        
        if (process == null || _monitoredProcess != null) return;
        
        _monitoredProcess = process;
        _monitoredProcess.Exited += MonitoredProcessOnExited;
        _monitoredProcess.EnableRaisingEvents = true;
        
        _searchProcessTimer.Stop();
        _playTimer.Start();
        
        _replayWatcher.Start();
        
        _logger.LogInformation("Found process");
    }

    private void MonitoredProcessOnExited(object? sender, EventArgs e)
    {
        _monitoredProcess?.Dispose();
        _monitoredProcess = null;
        
        _replayWatcher.Stop();
        
        _searchProcessTimer.Start();
        _playTimer.Stop();
        
        _logger.LogInformation("Process exited");
    }
}