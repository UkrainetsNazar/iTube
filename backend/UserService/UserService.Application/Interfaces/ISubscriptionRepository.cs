using Shared.Domain.ValueObjects;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetAsync(UserId subscriberId, ChannelId targetChannelId, CancellationToken ct = default);
    Task<bool> ExistsAsync(UserId subscriberId, ChannelId targetChannelId, CancellationToken ct = default);
    Task AddAsync(Subscription subscription, CancellationToken ct = default);
    Task<List<Subscription>> GetBySubscriberIdAsync(UserId subscriberId, CancellationToken ct = default);
    Task RemoveAsync(Subscription subscription, CancellationToken ct = default);
}