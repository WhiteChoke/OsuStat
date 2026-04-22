using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OsuStat.Core.Service.Interfaces;

namespace OsuStat.Core.Service.Impl;

public class ProcessMonitoringService : IProcessMonitoringService
{
    private readonly System.Timers.Timer _searchProcessTimer = new(2000);
    private readonly System.Timers.Timer _playTimer =  new(60000);
    private Process? _monitoredProcess;
    private readonly ILogger<ProcessMonitoringService> _logger;
    private readonly IReplayWatcher _replayWatcher;
    private const string ProcessName = "osu!";
    public event EventHandler? GameTimerElapsed; 
    
    public ProcessMonitoringService( 
        ILogger<ProcessMonitoringService> logger,
        IReplayWatcher replayWatcher
        ) 
    {
        _logger = logger;
        _replayWatcher = replayWatcher;
    }
    
    public void Run()
    {
        _searchProcessTimer.Elapsed += (sender, args) => CheckForProcess();
        _searchProcessTimer.Start();

        _playTimer.Elapsed += (sender, args) =>
        {
            GameTimerElapsed?.Invoke(sender, args);
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