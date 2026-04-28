using OsuStat.Data.Models;
using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.MVVM.Model;

public class PlayerStat : ObservableObject
{
    public double PlayTimeMin
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public int MapPlayed
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public double PpGained
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public double AvgBpm
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public double AvgStarRate
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public double AvgAccuracy
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public void LoadStatistics(PlayerStatEntity statEntity)
    {
        PlayTimeMin = statEntity.PlayTimeMin;
        MapPlayed = statEntity.MapPlayed;
        PpGained = Math.Round(statEntity.PpGained, 2); 
        AvgBpm = Math.Round(statEntity.AvgBpm, 2);
        AvgAccuracy = Math.Round(statEntity.AvgAccuracy, 2);
        AvgStarRate = Math.Round(statEntity.AvgStarRate, 2);
    }

    public void UpdateStatistic(PlayerStatEntity statEntity)
    {
        MapPlayed++;
        PpGained = Math.Round(statEntity.PpGained, 2); 
        AvgBpm = Math.Round(statEntity.AvgBpm, 2);
        AvgAccuracy = Math.Round(statEntity.AvgAccuracy, 2);
        AvgStarRate = Math.Round(statEntity.AvgStarRate, 2);
    }
}