using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public interface IDataService
{
    Task SaveDataAsync<T>(T data, string directory);
    Task UploadData(ObservableCollection<BeatMap> beatmaps);
}