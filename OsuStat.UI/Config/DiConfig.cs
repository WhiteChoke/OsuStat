namespace OsuStat.UI.Config;

public class RegisterDi
{
    public ServiceProvider GetServiceProvider(serviceProvider)
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
        services.AddSingleton<BestScore>();
        services.AddSingleton<ObservableCollection<BeatMap>>();
            
        return services.BuildServiceProvider();
    }
}