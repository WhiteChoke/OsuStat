using OsuParsers.Decoders;
using OsuStat.Core.Model;

namespace OsuStat.Core.Extractor;

public static class PlayerExtractor
{
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
}