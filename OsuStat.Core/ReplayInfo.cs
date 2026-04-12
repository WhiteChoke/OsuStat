using System.Net.Http.Json;
using OsuParsers.Beatmaps;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Enums.Database;
using OsuParsers.Replays;
using OsuStat.Core.Dto;
using OsuStat.Core.Dto.BeatmapDto;
using OsuStat.UI.Dto;

namespace OsuStat.Core;

public static class ReplayInfo
{
    private static readonly HttpClient HttpClient = new();
    private const string Url = "http://127.0.0.1:727/pp-calculate/beatmap/";

    public static async Task<ReplayResultDto?> Get(string replayPath, string osuDirectoryPath)
    {
        var osuDbPath = Path.Combine(osuDirectoryPath, "osu!.db");

        var osuDb = DatabaseDecoder.DecodeOsu(osuDbPath);
        var replay = ReplayDecoder.Decode(replayPath);

        var beatmap = osuDb.Beatmaps.Find(beatmap => replay.BeatmapMD5Hash == beatmap.MD5Hash) 
                      ?? throw new NullReferenceException("Beatmap not found!");

        var bgPath = Directory
            .GetFiles(Path.Combine(osuDirectoryPath, "Songs", beatmap.FolderName))
            .FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png"));

        try
        {
            var beatmapPath = string.Join("/", $"{osuDirectoryPath}/Songs/{beatmap.FolderName}/{beatmap.FileName}".Split('\\'));
            var replayStat = await GetReplayStat(beatmapPath, replay); 

            var ppGained =
                beatmap.RankedStatus.Equals(RankedStatus.Ranked)
                    ? replayStat.Pp
                    : 0.0;

            var mods = Enum.GetValues<Mods>()
                .Where(m => m != Mods.None && ((int)replay.Mods & (int)m) == (int)m)
                .ToList();
            
            var grade = CalculateGrade(replayStat.Acc, 
                replay.Count300, 
                replay.Count100, 
                replay.Count50,
                replay.CountMiss);

            var totalTime = TimeSpan
                .FromMilliseconds(beatmap.TotalTime)
                .ToString(@"mm\:ss");

            return new ReplayResultDto(
                beatmap.Title,
                beatmap.Artist,
                beatmap.Creator,
                replayStat.Beatmap.Bpm,
                totalTime,
                replayStat.Beatmap.Sr,
                replayStat.Beatmap.Hp,
                replayStat.Beatmap.Cs,
                replayStat.Beatmap.Ar,
                bgPath,
                ppGained,
                replay.Combo,
                replayStat.Acc,
                mods,
                grade
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

    }

    private static Grade CalculateGrade(double accuracy, ushort n300, ushort n100, ushort n50, ushort miss)
    {
        var total = n300 + n100 + n50 + miss;
        
        var percent300 = n300 * 100 / total;
        var percent50 = n50 * 100 / total;
        
        if (accuracy.Equals(100)) return Grade.SS;
        if ( percent300 > 90 && percent50 < 1 && miss == 0 ) return Grade.S;
        if ( (percent300 > 80 && miss == 0) || percent300 > 90 ) return Grade.A;
        if ( (percent300 > 70 && miss == 0) || percent300 > 80 ) return Grade.B;
        if ( percent300 > 60 ) return Grade.C;
        return Grade.D;
    }

    private static async Task<GetBeatMapStatResponseDto> GetReplayStat(string filePath, Replay replay)
    {
        var requestBody = new GetBeatMapStatRequestDto(
            filePath,
            replay.Count300,
            replay.Count100,
            replay.Count50,
            replay.CountMiss,
            replay.Combo,
            (short) replay.Mods
        );

        var httpResponse = await HttpClient.PostAsync(
            Url, 
            JsonContent.Create(requestBody)
        );
        var response = await httpResponse.Content.ReadFromJsonAsync<GetBeatMapStatResponseDto>();
        
        return response ?? throw new NullReferenceException($"Failed to create beatmap!\nResponse status: {httpResponse.StatusCode}");
    }

}