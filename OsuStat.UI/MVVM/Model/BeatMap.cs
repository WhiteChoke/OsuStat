using OsuStat.Core.Enums;

namespace OsuStat.UI.MVVM.Model;

public class BeatMap
{
    public string Name { get; set; }
    public string Artist { get; set; }
    public string Mapper { get; set; }
    public int PlayCount { get; set; }
    public double Bpm { get; set; }
    public string Length { get; set; }
    public double StarRate { get; set; }
    public double Hp { get; set; }
    public double Cs { get; set; }
    public double Ar { get; set; }
    public string BgPath { get; set; }
    public double PpGained { get; set; }
    public ushort MaxCombo  { get; set; }
    public string Date { get; set; }
    public double Accuracy { get; set; }
    public List<string> Mods { get; set; }
    public Grade Grade { get; set; }

    public BeatMap(
        string name, 
        string artist, string mapper, 
        double bpm, 
        string length, 
        double starRate, 
        double hp, 
        double cs, 
        double ar, 
        string bgPath,
        double ppGained, 
        ushort maxCombo,
        double accuracy,
        List<string> mods,
        Grade grade
        )
    {
        Name = name;
        Artist = artist;
        Mapper = mapper;
        Bpm = bpm;
        Length = length;
        StarRate = starRate;
        Hp = hp;
        Cs = cs;
        Ar = ar;
        BgPath = bgPath;
        PpGained = ppGained;
        MaxCombo = maxCombo;
        Accuracy = accuracy;
        Mods = mods;
        Grade = grade;
        
        Date = DateTime.Now.ToLongDateString();
    }

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