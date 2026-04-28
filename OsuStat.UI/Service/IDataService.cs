using OsuStat.Core.Model;

namespace OsuStat.UI.Service;

public interface IDataService
{
    void SaveAndUpdateAsyncEvent(object? sender, ReplayData replayData);
    Task LoadStatisticAsync();
    Task LoadUserInformationAsync();
}