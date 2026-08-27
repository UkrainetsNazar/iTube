using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Persistence.Configurations;

public sealed class VideoReactionConfiguration : IEntityTypeConfiguration<VideoReaction>
{
    public void Configure(EntityTypeBuilder<VideoReaction> builder)
    {
        builder.ToTable("VideoReactions");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasConversion(id => id.Value, value => new VideoReactionId(value)).ValueGeneratedNever();
        builder.Property(r => r.VideoId).HasConversion(id => id.Value, value => new VideoId(value)).IsRequired();
        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.Type).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(r => new { r.VideoId, r.UserId }).IsUnique();
    }
}