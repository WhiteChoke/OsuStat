using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsuStat.Data.Models;

namespace OsuStat.Data.Config;

public class PlayerStatConfig : IEntityTypeConfiguration<PlayerStatEntity>
{
    public void Configure(EntityTypeBuilder<PlayerStatEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.HasIndex(p => p.Date).IsUnique();
    }
}