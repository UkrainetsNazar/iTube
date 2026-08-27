using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion(id => id.Value, value => new CommentId(value)).ValueGeneratedNever();
        builder.Property(c => c.VideoId).HasConversion(id => id.Value, value => new VideoId(value)).IsRequired();
        builder.Property(c => c.AuthorId).IsRequired();
        builder.Property(c => c.Text).HasMaxLength(2000).IsRequired();
    }
}