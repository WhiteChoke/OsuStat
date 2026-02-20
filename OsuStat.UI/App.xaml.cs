using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.ViewModel;
using OsuStat.UI.Service;
using OsuStat.UI.Service.Impl;

namespace OsuStat.UI
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<SettingsViewModel>();
            
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            
            services.AddSingleton<Func<Type, ViewModel>>
            (
                provider => viewModelType => (ViewModel)provider.GetRequiredService(viewModelType)
            );
            
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _serviceProvider.GetRequiredService<INavigationService>().NavigateTo<HomeViewModel>();
            _serviceProvider.GetRequiredService<ISettingsService>();
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }

}
