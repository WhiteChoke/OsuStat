using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;
using System.IO;
using System.Text.Json;

namespace OsuStat.UI.Service.Impl
{
    public class SettingsService : ObservableObject ,ISettingsService
    {
        public string ApplicationFolder { get; }

        private readonly string _jsonPath;
        public string DataDirectoryPath { get; } = 
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Osu stat"
            );
        
        public string SavePlayerStatDirectoryPath { get; }
        public string SaveScoreDirectoryPath { get; }
        private Settings CurrentSettings { get; }

        public SettingsService()
        {
            // TODO: TEMP
            ApplicationFolder = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName;
            
            _jsonPath = Path.Combine(DataDirectoryPath, "settings.json");
            SavePlayerStatDirectoryPath = Path.Combine(DataDirectoryPath, "player");
            SaveScoreDirectoryPath = Path.Combine(DataDirectoryPath, "scores");
            
            try
            {
                CurrentSettings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(_jsonPath)) 
                                  ?? new Settings();
            }

            catch (IOException)
            {
                CreateSettingFile();
            }
            if (
                !Directory.Exists(SavePlayerStatDirectoryPath) 
                || !Directory.Exists(SaveScoreDirectoryPath)
                )
            {
                CreateSaveDirectories();
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

        public string GetGameFolder() { return CurrentSettings.GameFolder; }

        public string GetLanguage() { return CurrentSettings.Language; }

        private void CreateSettingFile()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Osu stat");
            File.WriteAllText(_jsonPath, JsonSerializer.Serialize(CurrentSettings));
        }
            
        
        private void CreateSaveDirectories()
        {
            Directory.CreateDirectory(SavePlayerStatDirectoryPath);
            Directory.CreateDirectory(SaveScoreDirectoryPath);
        }
    }
}
