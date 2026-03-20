using OsuParsers.Database;
using OsuParsers.Decoders;
using OsuStat.Core.Dto;
using OsuStat.UI.Dto;

namespace OsuStat.Core
{
    public static class PlayerInfo
    {
        public static PlayerInfoResponseDto GetPlayerInfo(string osuDirectoryPath)
        {
            var osuDbPath = Path.Combine(osuDirectoryPath, "osu!.db");
            var presenceDbPath = Path.Combine(osuDirectoryPath, "presence.db");
            
            var osuDb = DatabaseDecoder.DecodeOsu(osuDbPath);
            var presenceDb = DatabaseDecoder.DecodePresence(presenceDbPath);
                       
            var currentPlayer = presenceDb.Players.Find(player => player.Username.Equals(osuDb.PlayerName)) ;

            if (currentPlayer == null)
                throw new ArgumentNullException($"Not found user in presence with playername: {osuDb.PlayerName}");
            
            return new PlayerInfoResponseDto(currentPlayer.Username, currentPlayer.UserId, currentPlayer.Rank);
        }
    }
}
