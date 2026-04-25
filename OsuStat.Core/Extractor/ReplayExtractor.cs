using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuStat.Core.Enums;
using OsuStat.Core.Model;

namespace OsuStat.Core.Extractor;

public static class ReplayExtractor
{
    public static ReplayData Extract(string replayPath, string osuDirectoryPath)
    {
        var osuDbPath = Path.Combine(osuDirectoryPath, "osu!.db");
        
        var osuDb = DatabaseDecoder.DecodeOsu(osuDbPath);
        
        var replay = ReplayDecoder.Decode(replayPath);
        
        var dbBeatmap = osuDb.Beatmaps.Find(beatmap => replay.BeatmapMD5Hash == beatmap.MD5Hash) 
                        ?? throw new NullReferenceException("Beatmap not found!");
        
        var beatmapPath = Path.Combine(osuDirectoryPath, "Songs", dbBeatmap.FolderName, dbBeatmap.FileName);

        var beatmap = BeatmapDecoder.Decode(beatmapPath);
        
        var mods = Enum.GetValues<Mods>()
                 .Where(m => m != Mods.None && ((int)replay.Mods & (int)m) == (int)m)
                 .ToList();
        
        var length = TimeSpan
                 .FromMilliseconds(dbBeatmap.TotalTime)
                 .ToString(@"mm\:ss");

        var accuracy = CalculateAccuracy(
            n300: replay.Count300,
            n100: replay.Count100,
            n50: replay.Count50,
            nMiss: replay.CountMiss
            );
        
        var bpm = CalculateBpm(
            dbBeatmap.TimingPoints.First().BPM
            );

        var starRate = Math.Round(dbBeatmap.StandardStarRating[replay.Mods], 2);

        var grade = CalculateGrade(
            accuracy: accuracy,
            n300: replay.Count300,     
            n100: replay.Count100,     
            n50: replay.Count50,       
            nMiss: replay.CountMiss  
            );

        var bgPath = Path.Combine(osuDirectoryPath, "Songs", dbBeatmap.FolderName, beatmap.EventsSection.BackgroundImage);
        
        return new ReplayData
        {
            OnlineScoreId = replay.OnlineId,
            Name = dbBeatmap.Title,
            Artist = dbBeatmap.Artist,
            Mapper = dbBeatmap.Creator,
            StarRate = starRate,
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
            BeatmapHash = dbBeatmap.MD5Hash
        };
    }

    private static double CalculateAccuracy(
        ushort n300,
        ushort n100,
        ushort n50,
        ushort nMiss
        )
    {
        double a = 300.0 * n300 + 100.0 * n100 + 50.0 * n50;
        double b = 300.0 * (n300+n100+n50+nMiss);
        
        if (b == 0) return 0;
        
        var accuracy = a / b * 100.0;
        
        return Math.Round(accuracy, 2);
    }

    private static int CalculateBpm(double bpm)
    {
        return (int)Math.Ceiling(60 * 1000 / bpm);
    }
    
    private static Grade CalculateGrade(double accuracy, ushort n300, ushort n100, ushort n50, ushort nMiss)
    {
        var total = n300 + n100 + n50 + nMiss;
    
        var percent300 = n300 * 100 / total;
        var percent50 = n50 * 100 / total;
    
        if (accuracy.Equals(100)) return Grade.SS;
        if ( percent300 > 90 && percent50 < 1 && nMiss == 0 ) return Grade.S;
        if ( (percent300 > 80 && nMiss == 0) || percent300 > 90 ) return Grade.A;
        if ( (percent300 > 70 && nMiss == 0) || percent300 > 80 ) return Grade.B;
        if ( percent300 > 60 ) return Grade.C;
        return Grade.D;
    }
}