using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.Converters;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence.Configurations;

public sealed class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("UserAccounts");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion<UserIdConverter>()
            .ValueGeneratedNever();

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(u => u.CreatedAt).IsRequired();

        builder.OwnsOne(u => u.CurrentBan, banBuilder =>
        {
            banBuilder.Property(b => b.BannedByModeratorId)
                .HasConversion<UserIdConverter>()
                .HasColumnName("BanModeratorId");

            banBuilder.Property(b => b.Reason)
                .HasColumnName("BanReason")
                .HasMaxLength(500);

            banBuilder.Property(b => b.BannedAt)
                .HasColumnName("BannedAt");

            banBuilder.Property(b => b.ExpiresAt)
                .HasColumnName("BanExpiresAt");
        });

        builder.OwnsMany(u => u.History, historyBuilder =>
        {
            historyBuilder.ToTable("BanHistory");

            historyBuilder.WithOwner()
                .HasForeignKey("UserAccountId");

            historyBuilder.Property<Guid>("Id")
                .ValueGeneratedOnAdd();

            historyBuilder.HasKey("Id");

            historyBuilder.Property(h => h.BannedByModeratorId)
                .HasConversion<UserIdConverter>();

            historyBuilder.Property(h => h.UnbannedByModeratorId)
                .HasConversion<NullableUserIdConverter>();

            historyBuilder.Property(h => h.Reason)
                .HasMaxLength(500);

            historyBuilder.Property(h => h.BannedAt).IsRequired();
            historyBuilder.Property(h => h.ExpiresAt);
            historyBuilder.Property(h => h.UnbannedAt);
        });

        builder.Navigation(u => u.History)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(u => u.Version)
            .IsConcurrencyToken();

        builder.Ignore(u => u.DomainEvents);
    }
}