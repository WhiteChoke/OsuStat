using System.Net.Http.Json;
using OsuParsers.Beatmaps;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Enums.Database;
using OsuParsers.Replays;
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
        
        var beatmap = osuDb.Beatmaps.Find(beatmap => replay.BeatmapMD5Hash == beatmap.MD5Hash);
        
        if (beatmap == null)
            throw new NullReferenceException("Beatmap not found!");

        var bgPath = Directory
            .GetFiles(Path.Combine(osuDirectoryPath, "Songs", beatmap.FolderName))
            .FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png"));
        
        try
        {
            var requestBody = new GetBeatMapStatRequestDto(
                string.Join("/",$"{osuDirectoryPath}/Songs/{beatmap.FolderName}/{beatmap.FileName}".Split('\\')),
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
        
            if (response == null)
                throw new NullReferenceException($"Failed to create beatmap!\nResponse status: {httpResponse.StatusCode}");

            var ppGained = 
                beatmap.RankedStatus.Equals(RankedStatus.Ranked) 
                ? response.Pp 
                : 0.0;
            
            var mods = Enum.GetValues(typeof(Mods))
                .Cast<Mods>()
                .Where(m => m != Mods.None && ((int) replay.Mods & (int) m) == (int) m)
                .ToList();
            
            return new ReplayResultDto(
                beatmap.Title,
                beatmap.Artist,
                beatmap.Creator,
                response.Beatmap.Bpm,
                beatmap.TotalTime,
                response.Beatmap.Sr,
                response.Beatmap.Hp,
                response.Beatmap.Cs,
                response.Beatmap.Ar,
                bgPath,
                ppGained,
                replay.Combo,
                response.Acc,
                mods
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}