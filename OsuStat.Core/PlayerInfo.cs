using OsuParsers.Database;
using OsuParsers.Decoders;

namespace OsuStat.Core
{
    public class PlayerInfo
    {
        public async static Task<Dictionary<string, string>> GetPlayerInfo(string osuDirectoryPath)
        {
            var info = new Dictionary<string, string>();

            OsuDatabase osuDb = DatabaseDecoder.DecodeOsu(osuDirectoryPath + @"\osu!.db");
            PresenceDatabase presence = DatabaseDecoder.DecodePresence(osuDirectoryPath + @"\presence.db");
                       
            var currentPlayer = presence.Players.Find(player => player.Username.Equals(osuDb.PlayerName)) ;

            if (currentPlayer != null)
            {
                info.Add("Nickname", osuDb.PlayerName);
                info.Add("Id", currentPlayer.UserId.ToString());
                info.Add("GlobalRanking", currentPlayer.Rank.ToString());
            } 
            else
            {
                throw new ArgumentNullException($"Not found user in presence with playername: {osuDb.PlayerName}");
            }

            return info;
        }
    }
}
