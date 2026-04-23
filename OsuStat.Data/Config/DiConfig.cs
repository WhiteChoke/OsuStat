using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OsuStat.Data.Context;

namespace OsuStat.Data.Config;

public static class DiConfig
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddDbContext<OsuStatDbContext>();
        
        return services;
    }
}