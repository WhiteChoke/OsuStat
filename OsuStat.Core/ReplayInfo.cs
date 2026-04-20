using System.Net.NetworkInformation;
using OsuStat.Core.Extractor;
using OsuStat.Core.Model;

namespace OsuStat.Core;

public static class ReplayInfo
{
    public static async Task<ReplayData> Get(string replayPath, string osuDirectoryPath)
    {
        var replayData = ReplayExtractor.Extract(replayPath, osuDirectoryPath);

        if (NetworkInterface.GetIsNetworkAvailable())
        {
            try
            {
                await Requests.GetUserScores(replayData.OnlineScoreId);
            }
            catch (Exception e)
            {
                Console.WriteLine("gg");
            }
        }
        
        Console.WriteLine(replayData);
        
        return replayData;
    }
}