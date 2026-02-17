using OsuStat.UI.MVVM.Core;

namespace OsuStat.UI.Service
{
    public interface INavigationService
    {
        ViewModel CurrentView { get; }
        void NavigateTo<T>() where T : ViewModel;
    }
}
