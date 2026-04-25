namespace OsuStat.Core;

public static class Requests
{
    private static readonly HttpClient HttpClient = new();
    private const string Url = "https://a.ppy.sh/";

    
    public static async Task GetUserScores(long replayDataOnlineScoreId)
    {
        throw new NotImplementedException();
    }
    
    public static async Task<byte[]> GetAvatar(int id)
    {
        return await HttpClient.GetByteArrayAsync(Url + id);
    }
}