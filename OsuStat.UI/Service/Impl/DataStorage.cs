using System.Collections.ObjectModel;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class DataStorage : IDataStorage
{
    public BestScore BestScore { get; }
    public PlayerStat PlayerStat { get; }
    public ObservableCollection<BeatMap> Beatmaps { get; }
    public Player Player { get; }

    public DataStorage(
        BestScore bestScore,
        PlayerStat playerStat,
        ObservableCollection<BeatMap> beatmaps,
        Player player)
    {
        BestScore = bestScore;
        PlayerStat = playerStat;
        Beatmaps = beatmaps;
        Player = player;
    }
}