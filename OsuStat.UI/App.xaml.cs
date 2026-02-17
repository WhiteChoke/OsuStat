using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.View;
using OsuStat.UI.MVVM.ViewModel;

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
            services.AddSingleton<HomeView>();
            services.AddSingleton<SettingsView>();
            
            services.AddSingleton<IServiceCollection, ServiceCollection>();
            
            services.AddSingleton<Func<Type, ViewModel>>
            (
                provider => viewModelType => (ViewModel)provider.GetRequiredService(viewModelType)
            );
            
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }

}
