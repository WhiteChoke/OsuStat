using OsuStat.Core;
using OsuStat.UI.MVVM.Model;
using System.IO;
using System.Windows;

namespace OsuStat.UI.Service
{
    public class LoadUserData
    {
        public static async Task LoadData(Player user, string gameFolderPath ,string rootPath)
        {
            try
            {              
                var info = await PlayerInfo.GetPlayerInfo(gameFolderPath);

                user.Nickname = info["Nickname"];
                user.GlobalRanking = "#" + info["GlobalRanking"];

                string avatarPath = Path.Combine(rootPath, "Assets", "Avatar.jpeg");

                await LoadUserAvatar.Load(info["Id"], avatarPath);

                user.AvatarPath = avatarPath;

            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show($"Cannot find osu folder!\nPlease select folder: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
