using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.MVVM.Model
{
    public class Player : ObservableObject
    {
        private string _nickname = "Unknown";
        public string Nickname
        {
            get => _nickname;
            set 
            { 
                _nickname = value;
                OnPropertyChanged();
            }
        }
        
        private string _globalRanking = "0";
        public string GlobalRanking
        {
            get => _globalRanking;
            set 
            { 
                _globalRanking = value;
                OnPropertyChanged();
            }
        }
        
        private string _avatarPath = string.Empty;
        public string AvatarPath
        {
            get => _avatarPath;
            set 
            { 
                _avatarPath = value;
                OnPropertyChanged();
            }
        }

    }
}
