using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.MVVM.ViewModel;
using OsuStat.UI.Service;
using OsuStat.UI.Service.Impl;
using Serilog;
using Serilog.Core;

namespace OsuStat.UI
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            Log.Logger = GetLogger();
            
            IServiceCollection services = new ServiceCollection();

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            });
            
            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            });
            
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<SettingsViewModel>();
            
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IReplayWatcher, ReplayWatcher>();
            services.AddSingleton<IProcessMonitoringService, ProcessMonitoringService>();
            services.AddSingleton<IDataService, DataService>();
            
            services.AddSingleton<Func<Type, ViewModel>>
            (
                provider => viewModelType => (ViewModel)provider.GetRequiredService(viewModelType)
            );
            
            services.AddSingleton<PlayerStat>();
            services.AddSingleton<ObservableCollection<BeatMap>>();
            
            _serviceProvider = services.BuildServiceProvider();

            var applicationPath = _serviceProvider.GetRequiredService<ISettingsService>().ApplicationFolder;
            
            //TEMP
            Process.Start(Path.Combine(Directory.GetParent(applicationPath).FullName, "Endpoints" ,"api.exe"));
            Console.WriteLine("Application started");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _serviceProvider.GetRequiredService<INavigationService>().NavigateTo<HomeViewModel>();
            _serviceProvider.GetRequiredService<ISettingsService>();
            _serviceProvider.GetRequiredService<IProcessMonitoringService>().Run();
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Process.GetProcessesByName("api").FirstOrDefault()?.Kill();
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        private Logger GetLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Osu stat", "Logs", "log.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7)
                .CreateLogger();
        }
    }

}
