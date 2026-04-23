using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OsuStat.Data.Context;

public class DbContextFactory : IDesignTimeDbContextFactory<OsuStatDbContext>
{
    public OsuStatDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OsuStatDbContext>();
        optionsBuilder.UseSqlite("Data Source=osustat.db");

        return new OsuStatDbContext(optionsBuilder.Options);
    }
}