using OsuParsers.Database;
using OsuParsers.Decoders;
using static System.Net.WebRequestMethods;

namespace OsuStat.Core
{
    public class PlayerInfo
    {
        private readonly string _path;
        public PlayerInfo(string path)
        {
            _path = path;
        }
        public async Task<Dictionary<string, string>> GetPlayerInfo()
        {
            var info = new Dictionary<string, string>();

            OsuDatabase osuDb = DatabaseDecoder.DecodeOsu(_path + "osu!.db");
            PresenceDatabase presence = DatabaseDecoder.DecodePresence(_path + "presence.db");
            
            var currentPlayer = presence.Players.Find(player => player.Username.Equals(osuDb.PlayerName));

            info.Add("Nickname", osuDb.PlayerName);
            info.Add("Id", currentPlayer.UserId.ToString());
            info.Add("GlobalRanking", currentPlayer.Rank.ToString());

            return info;
        }
    }
}
