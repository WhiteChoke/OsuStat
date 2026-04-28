using System.Collections.ObjectModel;
using OsuStat.Core.Enums;
using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.MVVM.Model;

public class BeatMap : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Mapper { get; set; } = string.Empty;
    public int PlayCount { 
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public double Bpm { get; set; }
    public string Length { get; set; } = string.Empty;
    public double StarRate { get; set; }
    public double Hp { get; set; }
    public double Cs { get; set; }
    public double Ar { get; set; }
    public string BgPath { get; set; } = string.Empty;
    public ObservableCollection<Play> Plays { get; set; } = [];

    public override bool Equals(object? obj)
    {
        if (obj is not BeatMap other) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name &&
               Artist == other.Artist &&
               Mapper == other.Mapper &&
               StarRate.Equals(other.StarRate);
    }
}