using Microsoft.EntityFrameworkCore;
using OsuStat.Data.Context;
using OsuStat.Data.Models;

namespace OsuStat.Data.Repository;

public class BeatmapRepository
{
    private readonly OsuStatDbContext _context;
    
    public BeatmapRepository(OsuStatDbContext context)
    {
        _context = context;
    }


    public async Task<BeatmapEntity> CreateBeatmap(BeatmapEntity beatmap)
    {
        var saved = await _context.Beatmaps.AddAsync(beatmap);
        await _context.SaveChangesAsync();
        
        return saved.Entity;
    }

    public async Task<BeatmapEntity?> GetBeatmapsByHashCode(string hash)
    {
        return await _context.Beatmaps
            .AsNoTracking()
            .Where(b => b.BeatmapHash == hash)
            .FirstOrDefaultAsync();
    } 
}