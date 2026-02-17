using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.Service.Impl
{
    public class NavigationService : ObservableObject, INavigationService
    {
        private ViewModel _currentView;
        private readonly Func<Type, ViewModel> _viewModelFactory;

        public ViewModel CurrentView
        { 
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public NavigationService(Func<Type, ViewModel> viewFactory)
        {
            _viewModelFactory = viewFactory;
        }

        public void NavigateTo<T>() where T : ViewModel
        {
            ViewModel viewModel = _viewModelFactory.Invoke(typeof(T));
            CurrentView = viewModel;
        }
    }
}
