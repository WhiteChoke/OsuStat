using OsuStat.UI.MVVM.Model;
using System.ComponentModel;

namespace OsuStat.UI.Service
{
    public interface ISettingsService : INotifyPropertyChanged
    {
        public string ApplicationFolder { get; }
        void SetGameFolder(string folderPath);
        void SetLanguage();
        string GetGameFolder();
        string GetLanguage();
    }
}
