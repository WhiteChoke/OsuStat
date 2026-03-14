using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public interface IDataService
{
    void SaveScore(List<BeatMap> beatmap);
    void SaveStatistics(PlayerStat playerStat);
    void UploadData(ObservableCollection<BeatMap> beatmaps);
}