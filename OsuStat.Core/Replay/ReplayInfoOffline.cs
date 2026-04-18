using System.Net.Http.Json;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Enums.Database;
using OsuStat.Core.Dto;
using OsuStat.Core.Grade;
using OsuStat.UI.Dto;

namespace OsuStat.Core.Replay;

public class ReplayInfoOffline : IReplayInfo
{
    private readonly HttpClient _httpClient = new();
    private const string Url = "http://127.0.0.1:727/pp-calculate/beatmap/";

    public async Task<ReplayResultDto?> Get(string replayPath, string osuDirectoryPath)
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

            var mods = Enum.GetValues(typeof(Mods))
                .Cast<Mods>()
                .Where(m => m != Mods.None && ((int)replay.Mods & (int)m) == (int)m)
                .ToList();
            
            var grade = GradeCalculation.CalculateGrade(replayStat.Acc, 
                replay.Count300, 
                replay.Count100, 
                replay.Count50,
                replay.CountMiss);

            return new ReplayResultDto(
                beatmap.Title,
                beatmap.Artist,
                beatmap.Creator,
                replayStat.Beatmap.Bpm,
                beatmap.TotalTime,
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
    private async Task<GetBeatMapStatResponseDto> GetReplayStat(string filePath, OsuParsers.Replays.Replay replay)
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

        var httpResponse = await _httpClient.PostAsync(
            Url, 
            JsonContent.Create(requestBody)
        );
        var response = await httpResponse.Content.ReadFromJsonAsync<GetBeatMapStatResponseDto>();
        
        return response ?? throw new NullReferenceException($"Failed to create beatmap!\nResponse status: {httpResponse.StatusCode}");
    }

}