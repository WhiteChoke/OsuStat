using System.IO;
using System.Net.Http;
using System.Windows;

namespace OsuStat.UI.Service
{
    public class LoadUserAvatar
    {
        private static readonly HttpClient HttpClient = new();
        private const string Url = "https://a.ppy.sh/";
        public static async Task Load(string id, string path)
        {
            try
            {
                var pfpBytes = await HttpClient.GetByteArrayAsync(Url + id);
                await SetAvatarWithRetry(path, pfpBytes, 5);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Network error\n {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static async Task SetAvatarWithRetry(string path, byte[] pfpBytes, int retryCount)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    await File.WriteAllBytesAsync(path, pfpBytes);
                    return;
                }
                catch (IOException e)
                {
                    if (i == retryCount - 1)
                        MessageBox.Show($"Failed to set avatar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    await Task.Delay(500);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed to set avatar\n {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
