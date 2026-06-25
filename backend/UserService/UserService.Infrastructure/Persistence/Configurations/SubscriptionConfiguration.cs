using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.Converters;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SubscriberId)
            .HasConversion<UserIdConverter>()
            .IsRequired();

        builder.Property(s => s.TargetChannelId)
            .HasConversion<ChannelIdConverter>()
            .IsRequired();

        builder.HasIndex(s => new { s.SubscriberId, s.TargetChannelId }).IsUnique();

        builder.Property(s => s.IsMutual).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.Property(s => s.Version)
            .IsConcurrencyToken();

        builder.Ignore(s => s.DomainEvents);
    }
}