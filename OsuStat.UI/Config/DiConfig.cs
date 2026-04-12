using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuStat.UI.MVVM.Core;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.MVVM.View;
using OsuStat.UI.MVVM.ViewModel;
using OsuStat.UI.Service;
using OsuStat.UI.Service.Impl;
using Serilog;
using Serilog.Core;

namespace OsuStat.UI.Config;

public static class DiConfig
{
    public static ServiceProvider GetServiceProvider(Logger logger)
    {
        Log.Logger = logger;
            
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
        services.AddSingleton<BestScore>();
        services.AddSingleton<ObservableCollection<BeatMap>>();
            
        return services.BuildServiceProvider();
    }
}