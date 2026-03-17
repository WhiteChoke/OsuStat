using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.MVVM.Model;

public class PlayerStat : ObservableObject
{
    private double _playTimeMin = 0;
    public double PlayTimeMin
    {
        get => _playTimeMin;
        set
        {
            _playTimeMin = value;
            OnPropertyChanged();
        }
    }

    private int _mapPlayed = 0;
    public int MapPlayed
    {
        get => _mapPlayed;
        set
        {
            _mapPlayed = value;
            OnPropertyChanged();
        }
    }

    private double _ppGained = 0;
    public double PpGained
    {
        get => _ppGained;
        set
        {
            _ppGained = value;
            OnPropertyChanged();
        }
    }

    private double _totalBpm;
    private double _avgBpm = 0;
    public double AvgBpm
    {
        get => _avgBpm;
        set
        {
            _avgBpm = value;
            OnPropertyChanged();
        }
    }

    private double _totalStarRate;
    private double _avgStarRate = 0;
    public double AvgStarRate
    {
        get => _avgStarRate;
        set
        {
            _avgStarRate = value;
            OnPropertyChanged();
        }
    }

    private double _totalAccuracy;
    private double _avgAccuracy = 0;
    public double AvgAccuracy
    {
        get => _avgAccuracy;
        set
        {
            _avgAccuracy = value;
            OnPropertyChanged();
        }
    }

    public void Update(double bpm, double pp, double starRate, double accuracy)
    {
        MapPlayed++;
        PpGained += pp; 
        _totalBpm += bpm;
        _totalStarRate += starRate;
        _totalAccuracy += accuracy;
    
        AvgBpm = Math.Round(_totalBpm / MapPlayed, 2);
        AvgAccuracy = Math.Round(_totalAccuracy / MapPlayed, 2);
        AvgStarRate = Math.Round(_totalStarRate / MapPlayed, 2);
    }
}