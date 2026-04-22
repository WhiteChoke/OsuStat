using Microsoft.EntityFrameworkCore;
using OsuStat.Data.Config;
using OsuStat.Data.Models;

namespace OsuStat.Data;

public class OsuStatDbContext : DbContext
{
    public DbSet<BeatmapEntity>  Beatmaps { get; set; }
    public DbSet<PlayEntity>  Plays { get; set; }
    public DbSet<PlayerStatEntity>  PlayerStats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlayerStatConfig());
        modelBuilder.ApplyConfiguration(new BeatmapConfig());
        modelBuilder.ApplyConfiguration(new PlayConfig());
        
        base.OnModelCreating(modelBuilder);
    }
}