using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Replays;
using OsuStat.Core.Enums;
using OsuStat.Core.Model;
using OsuStat.Core.Model.Structures;

namespace OsuStat.Core.Extractor;

public static class ReplayExtractor
{
    public static async Task<ReplayData> Extract(string replayPath, string[] beatmapPath)
    {
        var osuFilePath = Path.Combine(beatmapPath);
        var folderPath = beatmapPath.First();
        
        var replay = ReplayDecoder.Decode(replayPath);
        
        var beatmap = BeatmapDecoder.Decode(osuFilePath);
        
        var mods = Enum.GetValues<Mods>()
                 .Where(m => m != Mods.None && ((int)replay.Mods & (int)m) == (int)m)
                 .ToList();
        
        var length = TimeSpan
                 .FromMilliseconds(beatmap.GeneralSection.Length)
                 .ToString(@"mm\:ss");

        var replayStat = ExtractReplayStat(replay);
        
        var accuracy = CalculateAccuracy(replayStat);
        
        var bpm = CalculateBpm(beatmap.TimingPoints.First().BeatLength);

        var apiStat = await Requests.GetMapStat(osuFilePath, replayStat, replay.Mods);
        
        var grade = CalculateGrade(
            accuracy: accuracy,
            stat: replayStat  
            );
        
        var bgPath = Path.Combine(folderPath, beatmap.EventsSection.BackgroundImage);
        
        return new ReplayData
        {
            OnlineScoreId = replay.OnlineId,
            Name = beatmap.MetadataSection.Title,
            Artist = beatmap.MetadataSection.Artist,
            Mapper = beatmap.MetadataSection.Creator,
            StarRate = Math.Round(apiStat.star_rate, 2),
            Hp = Math.Round(beatmap.DifficultySection.HPDrainRate, 2),
            Cs = Math.Round(beatmap.DifficultySection.CircleSize, 2),
            Ar = Math.Round(beatmap.DifficultySection.ApproachRate, 2),
            BgPath = bgPath,
            Mods = mods,
            Grade = grade,
            Combo = replay.Combo,
            Length = length,
            Accuracy = accuracy,
            Bpm = bpm,
            BeatmapId = beatmap.MetadataSection.BeatmapID,
            TimeStamp = replay.ReplayTimestamp,
            PpGained = Math.Round(apiStat.gained, 2)
        };
    }

    private static PlayStat ExtractReplayStat(Replay replay)
    {
        return new PlayStat
        {
           N300 = replay.Count300,
           N100 = replay.Count100,
           N50 = replay.Count50,
           Combo = replay.Combo,
           Misses = replay.CountMiss
        };
    }

    private static double CalculateAccuracy(
        PlayStat stat
        )
    {
        double a = 300.0 * stat.N300 + 100.0 * stat.N100 + 50.0 * stat.N50;
        double b = 300.0 * (stat.N300+stat.N100+stat.N50+stat.Misses);
        
        if (b == 0) return 0;
        
        var accuracy = a / b * 100.0;
        
        return Math.Round(accuracy, 2);
    }

    private static int CalculateBpm(double bpm)
    {
        return (int)Math.Ceiling(60 * 1000 / bpm);
    }
    
    private static Grade CalculateGrade(double accuracy, PlayStat stat)
    {
        var total = stat.N300 + stat.N100 + stat.N50 + stat.Misses;
    
        var percent300 = stat.N300 * 100 / total;
        var percent50 = stat.N50 * 100 / total;
    
        if (accuracy.Equals(100)) return Grade.SS;
        if ( percent300 > 90 && percent50 < 1 && stat.Misses == 0 ) return Grade.S;
        if ( (percent300 > 80 && stat.Misses == 0) || percent300 > 90 ) return Grade.A;
        if ( (percent300 > 70 && stat.Misses == 0) || percent300 > 80 ) return Grade.B;
        if ( percent300 > 60 ) return Grade.C;
        return Grade.D;
    }
}