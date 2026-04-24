using System.IO;
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
    } = string.Empty;

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
    } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images", "Best score.bg.jpg");
}