namespace OsuStat.Data.Models;

public class PlayerStatEntity
{
    public double PlayTimeMin { get; set; }
    public int MapPlayed { get; set; }
    public double PpGained { get; set; }
    public double AvgBpm { get; set; }
    public double AvgStarRate { get; set; }
    public double AvgAccuracy { get; set; }
    public DateTime Date { get; set; }
}