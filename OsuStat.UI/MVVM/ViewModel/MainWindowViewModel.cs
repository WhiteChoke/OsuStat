using OsuStat.UI.MVVM.Core;
using OsuStat.UI.Service;

namespace OsuStat.UI.MVVM.ViewModel
{
    public class MainWindowViewModel : Core.ViewModel
    {
        private INavigationService _navigation;

        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }
        
        public RelayCommand NavigateToSettingsCommand { get; set; }
        public MainWindowViewModel(INavigationService navigationService)
        {
            Navigation = navigationService;
            NavigateToSettingsCommand = new RelayCommand(o =>
                { Navigation.NavigateTo<SettingsViewModel>(); });
        }
    }
}
