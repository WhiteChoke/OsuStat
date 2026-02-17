using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;
using System.IO;


namespace OsuStat.UI.MVVM.ViewModel
{
    public class MainWindowViewModel : Core.ViewModel
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
