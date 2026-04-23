namespace OsuStat.Core.Service.Interfaces;

public interface IProcessMonitoringService
{
    void Run();
    public event EventHandler? GameTimerElapsed; 

}