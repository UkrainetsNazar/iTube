using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public sealed class SubscriptionRepository(UserDbContext dbContext) : ISubscriptionRepository
{
    public async Task AddAsync(Subscription subscription, CancellationToken ct = default) =>
        await dbContext.Subscriptions.AddAsync(subscription, ct);

    public async Task<bool> ExistsAsync(UserId subscriberId, ChannelId targetChannelId,
    CancellationToken ct = default) =>
        await dbContext.Subscriptions.AnyAsync(
            s => s.SubscriberId == subscriberId
            && s.TargetChannelId == targetChannelId,
            cancellationToken: ct);

    public async Task<Subscription?> GetAsync(UserId subscriberId, ChannelId targetChannelId,
    CancellationToken ct = default) =>
        await dbContext.Subscriptions
            .Where(s => s.SubscriberId == subscriberId && s.TargetChannelId == targetChannelId)
            .FirstOrDefaultAsync(cancellationToken: ct);

    public async Task<List<Subscription>> GetBySubscriberIdAsync(UserId subscriberId,
    CancellationToken ct = default) =>
        await dbContext.Subscriptions
            .Where(s => s.SubscriberId == subscriberId)
            .ToListAsync(cancellationToken: ct);

    public Task RemoveAsync(Subscription subscription, CancellationToken ct = default)
    {
        dbContext.Subscriptions.Remove(subscription);
        return Task.CompletedTask;
    }
}