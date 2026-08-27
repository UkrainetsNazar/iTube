namespace VideoService.Application.Interfaces;

public interface IUserSubscriptionRepository
{
    Task<IReadOnlyList<Guid>> GetSubscribedChannelIdsAsync(Guid subscriberId, CancellationToken ct);
}