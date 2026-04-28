using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using OsuStat.Core.Service.Interfaces;
using OsuStat.UI.Config;
using OsuStat.UI.MVVM.View;
using OsuStat.UI.MVVM.ViewModel;
using OsuStat.UI.Service;
using Serilog;

namespace OsuStat.UI
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            var logger = LoggerConfig.GetLogger();
            _serviceProvider = DiConfig.GetServiceProvider(logger);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger.Information("Application starting");

            RunApi();

            _serviceProvider.GetRequiredService<INavigationService>().NavigateTo<HomeViewModel>();
            _serviceProvider.GetRequiredService<IProcessMonitoringService>().Run();
            
            var dataService = _serviceProvider.GetRequiredService<IDataService>();

            Current.Dispatcher.InvokeAsync(async () =>
            {
                await dataService.LoadStatisticAsync();
                await dataService.LoadUserInformationAsync();
            });
            
            var replayWatcher = _serviceProvider.GetRequiredService<IReplayWatcher>();
            replayWatcher.OnReplayRegistered += dataService.SaveAndUpdateAsyncEvent;
            
            var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
            MapsterConfig.Configure(settingsService);
            settingsService.PropertyChanged += async (sender, args) =>
            {
                if (args.PropertyName == nameof(settingsService.SetGameFolder))
                {
                    await dataService.LoadUserInformationAsync();
                }
            };
            
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var process = Process.GetProcessesByName("api");
            foreach (var p in process)
            {
                p.Kill();
                p.WaitForExit();
            }
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        private void RunApi()
        {
            try
            {
                var applicationFolder = _serviceProvider.GetRequiredService<ISettingsService>().ApplicationFolder;
                Process.Start(Path.Combine(applicationFolder, "api.exe"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to run applications due to missing components\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Logger.Fatal("Failed to open api.exe");
            }
        }
    }
}
