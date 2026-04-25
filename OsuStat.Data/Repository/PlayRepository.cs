using Microsoft.EntityFrameworkCore;
using OsuStat.Data.Context;
using OsuStat.Data.Models;

namespace OsuStat.Data.Repository;

public class PlayRepository
{
    private readonly OsuStatDbContext _context;
    
    public PlayRepository(OsuStatDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlayEntity>> GetPlaysByDate(DateTime date)
    {
        var startDate = DateTime.Today;
        var endDate = startDate.AddDays(1);
        
        return await _context.Plays
            .AsNoTracking()
            .Include(p => p.Beatmap)
            .Where(p => p.PlayedAt >= startDate && p.PlayedAt < endDate)
            .ToListAsync();
    }

    public async Task<PlayEntity> CreatePlay(PlayEntity entity)
    {
        var saved = await _context.Plays.AddAsync(entity);
        await _context.SaveChangesAsync();
        return saved.Entity;
    }
}