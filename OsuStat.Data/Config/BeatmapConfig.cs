using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsuStat.Data.Models;

namespace OsuStat.Data.Config;

public class BeatmapConfig : IEntityTypeConfiguration<BeatmapEntity>
{
    public void Configure(EntityTypeBuilder<BeatmapEntity> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        
        builder.HasMany(b => b.Plays)
            .WithOne(p => p.Beatmap)
            .HasForeignKey(p => p.BeatmapId);
    }
}