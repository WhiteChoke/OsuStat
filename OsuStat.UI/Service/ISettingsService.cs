using OsuStat.UI.MVVM.Model;
using System.ComponentModel;

namespace OsuStat.UI.Service
{
    public interface ISettingsService : INotifyPropertyChanged
    {
        public string ApplicationFolder { get; }
        Settings CurrentSettings { get; }
        void SetGameFolder(string folderPath);
        void SetLanguage();
    }
}
