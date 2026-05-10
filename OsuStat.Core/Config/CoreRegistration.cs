using Microsoft.Extensions.DependencyInjection;
using OsuStat.Core.Model;
using OsuStat.Core.Service.Impl;
using OsuStat.Core.Service.Interfaces;

namespace OsuStat.Core.Config;

public static class CoreRegistration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IProcessMonitoringService, ProcessMonitoringService>();
        services.AddSingleton<IReplayWatcher, ReplayWatcher>();
        services.AddSingleton<GameWatcher>();
        services.AddSingleton<PlayedBeatmap>();
        return services;
    }
}