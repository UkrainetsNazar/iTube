using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;
using VideoService.Domain.Entities;
using VideoService.Domain.ValueObjects;

namespace VideoService.Infrastructure.Persistence.Configurations;

public sealed class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.ToTable("Videos");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, value => new VideoId(value))
            .ValueGeneratedNever();

        builder.Property(v => v.AuthorId).IsRequired();
        builder.Property(v => v.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(v => v.Visibility).HasConversion<string>().HasMaxLength(50);
        builder.Property(v => v.ThumbnailUrl).HasMaxLength(1000);

        builder.OwnsOne(v => v.Title, t => t.Property(x => x.Value).HasColumnName("Title").HasMaxLength(200).IsRequired());
        builder.OwnsOne(v => v.Description, d => d.Property(x => x.Value).HasColumnName("Description").HasMaxLength(5000));

        var tagsComparer = new ValueComparer<IReadOnlyList<Tag>>(
            (a, b) => a!.SequenceEqual(b!),
            a => a!.Aggregate(0, (hash, t) => HashCode.Combine(hash, t.Value)),
            a => (IReadOnlyList<Tag>)a!.ToList());

        builder.Property(v => v.Tags)
            .HasField("_tags")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                tags => tags.Select(t => t.Value).ToList(),
                values => values.Select(s => Tag.Create(s).Value).ToList())
            .Metadata.SetValueComparer(tagsComparer);

        builder.OwnsMany(v => v.Sources, source =>
        {
            source.ToTable("VideoSources");
            source.WithOwner().HasForeignKey("VideoId");
            source.HasKey(s => s.Id);
            source.Property(s => s.Resolution).HasConversion<string>().HasMaxLength(20);
            source.Property(s => s.Url).HasMaxLength(1000).IsRequired();
            source.Property(s => s.Format).HasMaxLength(20).IsRequired();
        });
    }
}