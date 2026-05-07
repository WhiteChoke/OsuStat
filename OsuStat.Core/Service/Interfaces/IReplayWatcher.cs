using OsuStat.Core.Model;

namespace OsuStat.Core.Service.Interfaces;

public interface IReplayWatcher
{
    void Start(string gameDirectory);
    void Stop();
    public event EventHandler<ReplayData>? OnReplayRegistered;
}