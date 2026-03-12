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
    } = 0;

    public int MapPlayed
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

    public double PpGained
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

    public double AvgBpm
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

    public double AvgStarRate
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

    public double AvgAccuracy
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = 0;

}