using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;

namespace OsuStat.UI.MVVM.ViewModel;

public class HomeViewModel : Core.ViewModel
{
    public Player User { get;}
    public PlayerStat PlayerStat { get; }
    public BestScore BestScore { get; } 
    public ObservableCollection<BeatMap> BeatMaps { get; }

    public HomeViewModel
    (
        IDataStorage dataStorage
        ) 
    {
        PlayerStat = dataStorage.PlayerStat;
        BestScore = dataStorage.BestScore;
        BeatMaps = dataStorage.Beatmaps;
        User = dataStorage.Player;
    }
    
}

