using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Persistence.Configurations;

public sealed class WatchHistoryEntryConfiguration : IEntityTypeConfiguration<WatchHistoryEntry>
{
    public void Configure(EntityTypeBuilder<WatchHistoryEntry> builder)
    {
        builder.ToTable("WatchHistory");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.UserId).IsRequired();
        builder.Property(w => w.VideoId).HasConversion(id => id.Value, value => new VideoId(value)).IsRequired();
        builder.HasIndex(w => new { w.UserId, w.VideoId }).IsUnique();
    }
}