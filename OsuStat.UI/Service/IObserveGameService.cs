using System.Collections.ObjectModel;
using System.ComponentModel;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service;

public interface IObserveGameService : INotifyPropertyChanged
{
    void Start(ObservableCollection<BeatMap> Beatmaps);
}