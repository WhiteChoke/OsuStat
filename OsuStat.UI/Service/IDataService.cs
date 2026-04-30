using OsuStat.Core.Model;

namespace OsuStat.UI.Service;

public interface IDataService
{
    void SaveAndUpdateAsyncEvent(object? sender, ReplayData replayData);
    void UpdateTimerAsyncEvent(object? sender, EventArgs eventArgs);
    Task LoadStatisticAsync();
    Task LoadUserInformationAsync();
}