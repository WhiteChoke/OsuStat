using System.IO;
using System.Net.Http;

namespace OsuStat.UI.Service
{
    public class LoadUserAvatar
    {
        private static readonly HttpClient _httpClient = new();
        private static readonly string _url = "https://a.ppy.sh/";
        public static async Task Load(string id, string path)
        {
            var pfpBytes = await _httpClient.GetByteArrayAsync(_url + id);
            await File.WriteAllBytesAsync(path, pfpBytes);
        }
    }
}
