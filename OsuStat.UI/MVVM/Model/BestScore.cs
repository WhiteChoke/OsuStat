using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.MVVM.Model;

public class BestScore : ObservableObject
{
    public string MapName
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public double Pp
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public string BgPath
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
}