using System.DirectoryServices.ActiveDirectory;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;
using System.IO;
using System.Text.Json;

namespace OsuStat.UI.Service.Impl
{
    public class SettingsService : ObservableObject ,ISettingsService
    {
        public string ApplicationFolder { get; }
        
        private readonly string _jsonPath = 
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Osu stat\settings.json";

        public Settings CurrentSettings { get; } = new();


        public SettingsService()
        {
            // TODO: TEMP
            ApplicationFolder = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            try
            {
                CurrentSettings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(_jsonPath));
            }
            catch (IOException)
            {
                CreateSettingFile();
            }
        }

        public void SetGameFolder(string folderPath)
        {
            CurrentSettings.GameFolder = folderPath;
            using var createStream = File.Create(_jsonPath);
            JsonSerializer.SerializeAsync(createStream, CurrentSettings);
            OnPropertyChanged();
        }

        public void SetLanguage()
        {
            throw new NotImplementedException();
        }

        private void CreateSettingFile()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Osu stat");
            File.WriteAllText(_jsonPath, JsonSerializer.Serialize(CurrentSettings));
        }
    }
}
