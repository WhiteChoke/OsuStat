using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.MVVM.Model;

public class PlayerStat : ObservableObject
{
    public double PlayTimeMin
    {
        get;
        set
        {
            field += value;
            OnPropertyChanged();
        }
    } = 0;
    
    public int MapPlayed
    {
        get;
        set
        {
            field++;
            OnPropertyChanged();
        }
    } = 0;
    public double PpGained
    {
        get;
        set
        {
            field = Math.Round(field + value, 1);
            OnPropertyChanged();
        }
    } = 0;

    private double _totalBpm;
    public double AvgBpm
    {
        get;
        set
        {
            _totalBpm += value;
            field = Math.Round(_totalBpm / MapPlayed, 1);
            OnPropertyChanged();
        }
    } = 0;

    private double _totalStarRate;
    public double AvgStarRate
    {
        get;
        set
        {
            _totalStarRate += value;
            field = Math.Round(_totalStarRate / MapPlayed, 1);
            OnPropertyChanged();
        }
    } = 0;

    private double _totalAccuracy;
    public double AvgAccuracy
    {
        get;
        set
        {
            _totalAccuracy += value;
            field = Math.Round(_totalAccuracy / MapPlayed, 1);
            OnPropertyChanged();
        }
    } = 0;

}