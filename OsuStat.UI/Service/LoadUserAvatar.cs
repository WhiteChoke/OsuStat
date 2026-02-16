using System.IO;
using System.Net.Http;
using System.Windows;

namespace OsuStat.UI.Service
{
    public class LoadUserAvatar
    {
        private static readonly HttpClient _httpClient = new();
        private static readonly string _url = "https://a.ppy.sh/";
        public async static Task Load(string id, string path)
        {
            byte[] pfpBytes = await _httpClient.GetByteArrayAsync(_url + id);
            await File.WriteAllBytesAsync(path, pfpBytes);
        }
    }
}
