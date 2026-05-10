namespace OsuStat.Core.Model;

public class PlayedBeatmap
{
    public long OsuBeatmapId { get; set; }
    public string OsuFileName { get; set; } = string.Empty;
    public string MapDirectory { get; set; } = string.Empty;
}