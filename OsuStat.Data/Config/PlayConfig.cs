using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsuStat.Data.Models;

namespace OsuStat.Data.Config;

public class PlayConfig : IEntityTypeConfiguration<PlayEntity>
{
    public void Configure(EntityTypeBuilder<PlayEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(p => p.Beatmap)
            .WithMany(b => b.Plays)
            .HasForeignKey(p => p.BeatmapId);
    }
}