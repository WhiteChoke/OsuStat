using System.Net.Http.Json;
using System.Net.NetworkInformation;
using OsuStat.Core.Extractor;


namespace OsuStat.Core.Replay;

public class ReplayInfoOffline
{
    
    public async Task<ReplayData> Get(string replayPath, string osuDirectoryPath)
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
        
        return replayData;
    }
    public static void Main(string[] args)
    {
        
        
        var replayInfoOnline = new ReplayInfoOffline();
        var rep = @"C:\Users\denis\OneDrive\Desktop\solo-replay-osu_4872484_4095578285.osr";
        var dir = @"D:\osu!";
        Task.Run(async () =>
        {
            var data = await replayInfoOnline.Get(rep, dir);
            Console.WriteLine(data.ToString());
        }).Wait();
    }

}