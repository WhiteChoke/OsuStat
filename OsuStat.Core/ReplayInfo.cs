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
        var osuDb = DatabaseDecoder.DecodeOsu(Path.Combine(gamePath, "osu!.db"));
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
        
        var httpResponse = await _httpClient.PostAsync(
            _url, 
            JsonContent.Create(requestBody)
            );
        var response = await httpResponse.Content.ReadFromJsonAsync<GetBeatMapStatResponseDto>();

        if (response != null)
        {
            result.Add("Name", beatmap.FileName);
            result.Add("Artist", beatmap.Artist);
            result.Add("Mapper", beatmap.Creator);
            result.Add("Bpm", response.beatmap.Bpm);
            result.Add("Length", replay.ReplayLength);
            result.Add("StarRate", response.beatmap.Sr);
            result.Add("Hp", response.beatmap.Hp);
            result.Add("Cs", response.beatmap.Cs);
            result.Add("Ar", response.beatmap.Ar);
            result.Add("BgPath", bgPath[0]);
        }
        else
        {
            throw new NullReferenceException("Beatmap not found");
        }
      
        return result;
    }
}