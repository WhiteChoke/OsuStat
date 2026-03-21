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
    private double _ppGained = 0;
    public double PpGained
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    private double _totalBpm;
    public double AvgBpm
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    private double _totalStarRate;
    public double AvgStarRate
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    private double _totalAccuracy;
    public double AvgAccuracy
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public void Update(double bpm, double pp, double starRate, double accuracy)
    {
        MapPlayed++;
        _ppGained += pp;
        _totalBpm += bpm;
        _totalStarRate += starRate;
        _totalAccuracy += accuracy;
    
        PpGained = Math.Round(_ppGained, 2); 
        AvgBpm = Math.Round(_totalBpm / MapPlayed, 2);
        AvgAccuracy = Math.Round(_totalAccuracy / MapPlayed, 2);
        AvgStarRate = Math.Round(_totalStarRate / MapPlayed, 2);
    }
}