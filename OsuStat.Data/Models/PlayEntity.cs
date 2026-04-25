using OsuParsers.Enums;
using OsuStat.Core.Enums;

namespace OsuStat.Data.Models;

public class PlayEntity
{
    public long Id { get; set; }
    public double PpGained { get; set; } 
    public ushort Combo  { get; set; }
    public double Accuracy { get; set; }
    public List<Mods> Mods { get; set; } = [];
    public Grade Grade { get; set; }
    public DateTime PlayedAt { get; set; }
    public long BeatmapId { get; set; }
    public BeatmapEntity? Beatmap { get; set; }
}
