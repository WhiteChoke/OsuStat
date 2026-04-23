namespace OsuStat.Core.Service.Interfaces;

public interface IReplayWatcher
{
    void Start();
    void Stop();
    public event EventHandler? OnReplayRegistered;
}