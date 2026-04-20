using System.Net.Http.Json;

namespace OsuStat.Core.Replay;

public static class Requests
{
    private static readonly HttpClient HttpClient = new();
    
    public static async Task GetUserScores(long replayDataOnlineScoreId)
    {
        throw new NotImplementedException();
    }
}