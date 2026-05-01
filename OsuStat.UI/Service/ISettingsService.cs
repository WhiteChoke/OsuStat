using System.ComponentModel;

namespace OsuStat.UI.Service
{
    public interface ISettingsService : INotifyPropertyChanged
    {
        public string ApplicationFolder { get; }
        public string ModIconsFolder { get; }
        void SetGameFolder(string folderPath);
        string GameFolder { get; }
    }
}
