using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service;

public interface IDataService
{
    Task SaveDataAsync<T>(T data, string directory);
    Task LoadStatisticAsync(ObservableCollection<BeatMap> beatmaps);
    Task LoadUserInformationAsync(Player player);
}