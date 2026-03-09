using OsuParsers.Decoders;
using OsuParsers.Replays;

namespace OsuStat.Core;

public class ReplayInfo
{
    public static Dictionary<string, string> Get(string replayPath, string gamePath)
    {
        var result = new Dictionary<string, string>();
        var OsuDb = DatabaseDecoder.DecodeOsu(@"D:\osu!\osu!.db");
        var replay = ReplayDecoder.Decode(replayPath);
        var beatmap = OsuDb.Beatmaps.Find(beatmap => replay.BeatmapMD5Hash == beatmap.MD5Hash);
        var bgPath = Directory.GetFiles(Path.Combine(gamePath, "Songs", beatmap.FolderName), "*.jpg");
        result.Add("Name", beatmap.Title);
        result.Add("Artist", beatmap.Artist);
        result.Add("Mapper", beatmap.Creator);
        result.Add("Length", replay.ReplayLength.ToString());
        result.Add("StarRate", beatmap.Difficulty);
        result.Add("Hp", beatmap.HPDrain.ToString());
        result.Add("Cs", beatmap.CircleSize.ToString());
        result.Add("Ar", beatmap.ApproachRate.ToString());
        result.Add("BgPath",  bgPath[0]);
        
        
        return result;
    }
}