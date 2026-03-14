using OsuStat.UI.MVVM.Model;
using System.ComponentModel;

namespace OsuStat.UI.Service
{
    public interface ISettingsService : INotifyPropertyChanged
    {
        public string DataDirectoryPath { get; }
        public string ApplicationFolder { get; }
        public string SavePlayerStatDirectoryPath { get; }
        public string SaveScoreDirectoryPath { get; }
        void SetGameFolder(string folderPath);
        void SetLanguage();
        string GetGameFolder();
        string GetLanguage();
    }
}
