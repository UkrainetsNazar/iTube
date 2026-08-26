using MediaService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.ValueObjects;

namespace MediaService.Infrastructure.Persistence.Configurations;

public sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("MediaAssets");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new MediaAssetId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.OriginalFileName).HasMaxLength(500).IsRequired();
        builder.Property(x => x.RawStoragePath).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.ThumbnailStoragePath).HasMaxLength(1000);
        builder.Property(x => x.FailureReason).HasMaxLength(2000);
        builder.Property(x => x.MediaType).HasConversion<string>().HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);

        builder.OwnsMany(x => x.Variants, variant =>
        {
            variant.ToTable("MediaVariants");
            variant.WithOwner().HasForeignKey("MediaAssetId");
            variant.HasKey(v => v.Id);
            variant.Property(v => v.Resolution).HasConversion<string>().HasMaxLength(20);
            variant.Property(v => v.StoragePath).HasMaxLength(1000).IsRequired();
        });
    }
}