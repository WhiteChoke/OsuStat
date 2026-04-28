using System.Collections.ObjectModel;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuStat.Core.Config;
using OsuStat.Data.Config;
using OsuStat.UI.Mapper;
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

        services.AddSingleton<IDataStorage, DataStorage>();
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
        services.AddSingleton<IDataService, DataService>();
        services.AddCoreServices(); 
            
        services.AddSingleton<Func<Type, ViewModel>>
        (
            provider => viewModelType => (ViewModel)provider.GetRequiredService(viewModelType)
        );
            
        services.AddSingleton<Player>();
        services.AddSingleton<PlayerStat>();
        services.AddSingleton<BestScore>();
        services.AddSingleton<ObservableCollection<BeatMap>>();
        services.AddSingleton<BeatmapMapper>();
        
        services.AddDataServices();
        
        services.AddMapster();
            
        return services.BuildServiceProvider();
    }
}