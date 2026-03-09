using System.Net.Http.Json;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Replays;
using OsuStat.UI.Dto;

namespace OsuStat.Core;

public class ReplayInfo
{
    private static readonly HttpClient _httpClient = new();
    private static readonly string _url = "http://127.0.0.1:727/pp-calculate/beatmap/";
    
    public static async Task<Dictionary<string, object>> Get(string replayPath, string gamePath)
    {
        var result = new Dictionary<string, object>();
        var osuDb = DatabaseDecoder.DecodeOsu(@"D:\osu!\osu!.db");
        var replay = ReplayDecoder.Decode(replayPath);
        var beatmap = osuDb.Beatmaps.Find(beatmap => replay.BeatmapMD5Hash == beatmap.MD5Hash);
        var bgPath = Directory.GetFiles(Path.Combine(gamePath, "Songs", beatmap.FolderName), "*.jpg");

        var requestBody = new GetBeatMapStatRequestDto(
            $"{gamePath}/Songs/{beatmap.FolderName}/{beatmap.FileName}",
            replay.Count300,
            replay.Count100,
            replay.Count50,
            replay.CountMiss,
            replay.Combo,
            0
            );
        
        var response = await _httpClient.PostAsync(
            _url, 
            JsonContent.Create(requestBody)
            );
        
        var r = await response.Content.ReadFromJsonAsync<GetBeatMapStatResponseDto>();
      
        return result;
    }
}