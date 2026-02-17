using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.MVVM.Model
{
    public class Player : ObservableObject
    {
        private string _nickname;

        public string Nickname
        {
            get { return _nickname; }
            set 
            { 
                _nickname = value;
                OnPropertyChanged();
            }
        }
        private string _globalRanking;

        public string GlobalRanking
        {
            get { return _globalRanking; }
            set 
            { 
                _globalRanking = value;
                OnPropertyChanged();
            }
        }

        private string _avatarPath;
        public string AvatarPath
        {
            get { return _avatarPath; }
            set 
            {
                _avatarPath = value;
                OnPropertyChanged();
            }
        }

    }
}
