using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.Converters;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;

namespace UserService.Infrastructure.Persistence.Configurations;

public sealed class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("Channels");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion<ChannelIdConverter>()
            .ValueGeneratedNever();

        builder.Property(c => c.OwnerId)
            .HasConversion<UserIdConverter>()
            .IsRequired();

        builder.HasIndex(c => c.OwnerId).IsUnique();

        builder.Property(c => c.Name)
            .HasConversion(
                name => name.Value,
                value => ChannelName.Create(value).Value)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasConversion(
                desc => desc != null ? desc.Value : null,
                value => value != null ? ChannelDescription.Create(value).Value : null)
            .HasMaxLength(1000);

        builder.OwnsOne(c => c.Avatar, avatarBuilder =>
        {
            avatarBuilder.Property(a => a.Bucket)
                .HasColumnName("AvatarBucket")
                .HasMaxLength(100);

            avatarBuilder.Property(a => a.Key)
                .HasColumnName("AvatarKey")
                .HasMaxLength(500);

            avatarBuilder.Property(a => a.Url)
                .HasColumnName("AvatarUrl")
                .HasMaxLength(1000);
        });

        builder.OwnsOne(c => c.Banner, bannerBuilder =>
        {
            bannerBuilder.Property(b => b.Bucket)
                .HasColumnName("BannerBucket")
                .HasMaxLength(100);

            bannerBuilder.Property(b => b.Key)
                .HasColumnName("BannerKey")
                .HasMaxLength(500);

            bannerBuilder.Property(b => b.Url)
                .HasColumnName("BannerUrl")
                .HasMaxLength(1000);
        });

        builder.Property(c => c.SubscribersCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(c => c.VideoCount)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.CreatedAt).IsRequired();

        builder.Property(c => c.Version)
            .IsConcurrencyToken();

        builder.Ignore(c => c.DomainEvents);
    }
}