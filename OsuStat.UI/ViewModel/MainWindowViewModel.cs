using OsuStat.UI.Model;
using OsuStat.UI.MVVM;
using OsuStat.UI.Service;
using System.IO;


namespace OsuStat.UI.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyBase
    {
        public Player User { get; set; } = new Player();

        private readonly string _rootDirectory = Directory.GetParent(".").Parent.Parent.ToString();

        public MainWindowViewModel()
        {
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadUserData.LoadData(User, _rootDirectory);
        }
    }
}
