namespace OsuStat.Core.Model;

public class CurrentBeatmap
{
    public long OsuBeatmapId { get; set; }
    public string Name { get; set; }= string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Mapper { get; set; } = string.Empty;
    public double Bpm { get; set; }
    public string Length { get; set; } = "0:00";
    public double StarRate { get; set; }
    public double Hp { get; set; }
    public double Cs { get; set; }
    public double Ar { get; set; }
}