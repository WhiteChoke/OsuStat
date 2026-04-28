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

    public async Task<PlayerStatEntity?> GetStatByDateAsync(DateTime date)
    {
        return await _context.PlayerStats
            .AsNoTracking()
            .FirstOrDefaultAsync(stat => stat.Date == date);
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
                .SetProperty(s => s.SessionStarRateSum, playerStat.SessionStarRateSum)
                .SetProperty(s => s.SessionAccuracySum, playerStat.SessionAccuracySum)
                .SetProperty(s => s.SessionBpmSum, playerStat.SessionBpmSum)
                );
    }

    public async Task<PlayerStatEntity> CreateStat()
    {
        var current = await _context.PlayerStats.AsNoTracking()
            .FirstOrDefaultAsync(stat => stat.Date == DateTime.Today);

        if (current != null)
            return current;
        
        var entity = new PlayerStatEntity
        {
            Date = DateTime.Today
        };

        var saved = await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        
        return saved.Entity;
    }
}