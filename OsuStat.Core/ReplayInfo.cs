using System.Net.Http.Json;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Replays;
using OsuStat.UI.Dto;

namespace OsuStat.Core;

public class ReplayInfo
{
    private static readonly HttpClient HttpClient = new();
    private const string Url = "http://127.0.0.1:727/pp-calculate/beatmap/";
    
    public static async Task<ReplayResultDto?> Get(string replayPath, string gamePath)
    {
        var osuDb = DatabaseDecoder.DecodeOsu(Path.Combine(gamePath, "osu!.db"));
        var replay = ReplayDecoder.Decode(replayPath);
        var beatmap = osuDb.Beatmaps.Find(beatmap => replay.BeatmapMD5Hash == beatmap.MD5Hash);
        var bgPath = Directory.GetFiles(Path.Combine(gamePath, "Songs", beatmap.FolderName), "*.jpg");

        var requestBody = new GetBeatMapStatRequestDto(
            string.Join("/",$"{gamePath}/Songs/{beatmap.FolderName}/{beatmap.FileName}".Split('\\')),
            replay.Count300,
            replay.Count100,
            replay.Count50,
            replay.CountMiss,
            replay.Combo,
            0
            );

        try
        {
            var httpResponse = await HttpClient.PostAsync(
                Url, 
                JsonContent.Create(requestBody)
            );
            var response = await httpResponse.Content.ReadFromJsonAsync<GetBeatMapStatResponseDto>();
        
            return new ReplayResultDto(
                beatmap.FileName,
                beatmap.Artist,
                beatmap.Creator,
                response.beatmap.Bpm,
                replay.ReplayLength,
                response.beatmap.Sr,
                response.beatmap.Hp,
                response.beatmap.Cs,
                response.beatmap.Ar,
                bgPath.Length != 0 ? bgPath.First() : "",
                response.Pp,
                replay.Combo
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}