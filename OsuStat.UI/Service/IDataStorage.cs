using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service;

public interface IDataStorage
{
    BestScore BestScore { get; }
    PlayerStat PlayerStat { get; }
    Player Player { get; }
    ObservableCollection<BeatMap> Beatmaps { get; }
}