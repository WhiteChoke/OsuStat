using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OsuStat.Data.Context;
using OsuStat.Data.Repository;

namespace OsuStat.Data.Config;

public static class DiConfig
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddDbContext<OsuStatDbContext>(options => options.UseSqlite( "Data Source=osustat.db"));
        services.AddSingleton<PlayerStatRepository>();
        
        using (var scope = services.BuildServiceProvider().CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<OsuStatDbContext>();
            db.Database.EnsureCreated();
        }
        
        return services;
    }
}