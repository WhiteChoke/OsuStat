namespace OsuStat.Data.Models;

public class PlayerStatEntity
{
    public long Id { get; set; }
    public double PlayTimeMin { get; set; }
    public int MapPlayed { get; set; }
    public double PpGained { get; set; }
    public double AvgBpm { get; set; }
    public double AvgStarRate { get; set; }
    public double AvgAccuracy { get; set; }
    public double SessionBpmSum { get; set; }     
    public double SessionStarRateSum { get; set; }
    public double SessionAccuracySum { get; set; }
    public DateTime Date { get; set; }
}