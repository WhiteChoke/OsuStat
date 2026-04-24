using Microsoft.EntityFrameworkCore;
using OsuStat.Data.Context;
using OsuStat.Data.Models;

namespace OsuStat.Data.Repository;

public class PlayerStatRepository
{
    private readonly OsuStatDbContext _context;
    
    public PlayerStatRepository(OsuStatDbContext context)
    {
        _context = context;
    }

    public async Task<PlayerStatEntity?> GetTodayStatAsync()
    {
        return await _context.PlayerStats
            .AsNoTracking()
            .FirstOrDefaultAsync(stat => stat.Date == DateTime.Today);
    }

    public async Task UpdateTodayStatAsync(PlayerStatEntity playerStat)
    {
        await _context.PlayerStats
            .Where(stat => stat.Date == DateTime.Today)
            .ExecuteUpdateAsync(stat => stat
                .SetProperty(s => s.AvgAccuracy, playerStat.AvgAccuracy)
                .SetProperty(s => s.AvgStarRate, playerStat.AvgStarRate)
                .SetProperty(s => s.AvgBpm, playerStat.AvgBpm)
                .SetProperty(s => s.MapPlayed, playerStat.MapPlayed)
                .SetProperty(s => s.PpGained, playerStat.PpGained)
                );
    }

    public async Task CreateStat()
    {
        var current = await _context.PlayerStats.AsNoTracking()
            .FirstOrDefaultAsync(stat => stat.Date == DateTime.Today);

        if (current == null)
        {
            var entity = new PlayerStatEntity
            {
                Date = DateTime.Today
            };
            
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        }
    }
}