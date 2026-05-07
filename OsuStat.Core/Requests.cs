using System.Net.Http.Json;
using OsuParsers.Enums;
using OsuStat.Core.Model;
using OsuStat.Core.Model.Dto;

namespace OsuStat.Core;

public static class Requests
{
    private static readonly HttpClient HttpClient = new();
    private const string Url = "https://a.ppy.sh/";
    private const string ApiUrl = "http://127.0.0.1:7272/";

    
    public static async Task<GetMapStatResponseDto> GetMapStat(string filePath, PlayStat stat, Mods mods)
    {
        var requestBody = new GetMapStatRequestDto(
            filePath: ConvertPath(filePath),
            n300: stat.N300,
            n100: stat.N100,
            n50: stat.N50,
            combo: stat.Combo,
            misses: stat.Misses,
            mods: (int) mods
            );
        
        var response = await HttpClient.PostAsJsonAsync(ApiUrl + "offline/result", requestBody );
        
        if (!response.IsSuccessStatusCode)
           throw new HttpRequestException(response.ReasonPhrase);
        
        return await response.Content.ReadFromJsonAsync<GetMapStatResponseDto>()
               ?? throw new NullReferenceException($"Failed to create beatmap!\nResponse status: {response.StatusCode}");
    }
    
    public static async Task<byte[]> GetAvatar(int id)
    {
        return await HttpClient.GetByteArrayAsync(Url + id);
    }

    private static string ConvertPath(string filePath)
    {
        return string.Join("/", filePath.Split('\\'));
    }
}