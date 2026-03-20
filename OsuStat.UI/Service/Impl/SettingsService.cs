using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace OsuStat.UI.Service.Impl
{
    public class SettingsService : ObservableObject ,ISettingsService
    {
        private readonly ILogger<SettingsService> _logger;
        public string ApplicationFolder { get; }

        private readonly string _jsonPath;
        public string DataDirectoryPath { get; } = 
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Osu stat"
            );
        public string SavePlayerStatDirectoryPath { get; }
        public string SaveScoreDirectoryPath { get; }
        public string GameFolder => CurrentSettings.GameFolder;
        public string Language => CurrentSettings.Language;
        public string ModIconsFolder { get; }
        private Settings CurrentSettings { get; }

        public SettingsService(ILogger<SettingsService> logger)
        {
            _logger = logger;
            ApplicationFolder = AppContext.BaseDirectory;
            _jsonPath = Path.Combine(DataDirectoryPath, "Settings.json");
            SavePlayerStatDirectoryPath = Path.Combine(DataDirectoryPath, "Player");
            SaveScoreDirectoryPath = Path.Combine(DataDirectoryPath, "Scores");
            ModIconsFolder = Path.Combine(ApplicationFolder, "Assets", "Images", "Mod icons");
            
            Directory.CreateDirectory(DataDirectoryPath);
            Directory.CreateDirectory(SavePlayerStatDirectoryPath);
            Directory.CreateDirectory(SaveScoreDirectoryPath);
            Directory.CreateDirectory(Path.Combine(SaveScoreDirectoryPath, "Best"));
            
            
            if (File.Exists(_jsonPath))
            {
                try
                {
                    CurrentSettings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(_jsonPath))
                        ?? new Settings();
                    _logger.LogInformation("Settings file loaded");
                }
                catch (IOException e)
                {
                    CurrentSettings = new Settings();
                    _logger.LogError("Failed to load settings: {}", e.Message);
                }
            }
            else
            {
                CurrentSettings = new Settings();
                File.WriteAllText(_jsonPath, JsonSerializer.Serialize(CurrentSettings));
                _logger.LogInformation("Created new settings file");
            }
        }

        public void SetGameFolder(string folderPath)
        {
            CurrentSettings.GameFolder = folderPath;
            
            var json = JsonSerializer.Serialize(CurrentSettings);
            File.WriteAllText(_jsonPath, json);
            
            OnPropertyChanged();
            _logger.LogInformation("Game folder changed");
        }

        public void SetLanguage()
        {
            throw new NotImplementedException();
        }
    }
}
