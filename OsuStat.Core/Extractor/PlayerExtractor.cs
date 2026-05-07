using System.Text.RegularExpressions;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using OsuParsers.Decoders;
using OsuStat.Core.Model;

namespace OsuStat.Core.Extractor;

public static class PlayerExtractor
{
    private const short RetryCount = 5;
    
    public static PlayerData Extract(string osuDirectoryPath)
    {
        var osuDbPath = Path.Combine(osuDirectoryPath, "osu!.db");
        var presenceDbPath = Path.Combine(osuDirectoryPath, "presence.db");
            
        var osuDb = DatabaseDecoder.DecodeOsu(osuDbPath);
        var presenceDb = DatabaseDecoder.DecodePresence(presenceDbPath);
                       
        var currentPlayer = presenceDb.Players.Find(player => player.Username.Equals(osuDb.PlayerName));

        if (currentPlayer == null)
            throw new ArgumentNullException($"Not found user in presence with playername: {osuDb.PlayerName}");

        return new PlayerData
        {
            Nickname = currentPlayer.Username,
            Id = currentPlayer.UserId,
            Ranking = currentPlayer.Rank,
            LastSeenAt = currentPlayer.LastUpdateTime,
        };
    }
    
    public static int ExtractPp(StructuredOsuMemoryReader reader)
    {
        short retryCount = 0;
        BanchoUser banchoUser = new();

        while (retryCount < RetryCount)
        {
            if (reader.TryRead(banchoUser))
            {
                var rawPp = Regex.Match(banchoUser.UserPpAccLevel, "[\\d,.]+").Value;
                var pp = rawPp.Replace(",", "");
                
                return int.Parse(pp);
            }
            retryCount++;
        }
        return 0;
    }
}