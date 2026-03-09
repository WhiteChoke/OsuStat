namespace OsuStat.UI.MVVM.Model;

public class BeatMap
{
    public string Name { get; set; }
    public string Artist { get; set; }
    public string Mapper { get; set; }
    public int PlayCount { get; set; }
    public int Bpm { get; set; }
    public int Length { get; set; }
    public double StarRate { get; set; }
    public double Hp { get; set; }
    public double Cs { get; set; }
    public double Ar { get; set; }
    public string BgPath { get; set; }

    public BeatMap(
        string name, 
        string artist, 
        string mapper, 
        int playCount, 
        int bpm, int length, 
        double starRate, 
        double hp, 
        double cs, 
        double ar, 
        string bgPath)
    {
        Name = name;
        Artist = artist;
        Mapper = mapper;
        PlayCount = playCount;
        Bpm = bpm;
        Length = length;
        StarRate = starRate;
        Hp = hp;
        Cs = cs;
        Ar = ar;
        BgPath = bgPath;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BeatMap other) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name &&
               Artist == other.Artist &&
               Mapper == other.Mapper &&
               StarRate.Equals(other.StarRate) &&
               BgPath == other.BgPath;
    }
}