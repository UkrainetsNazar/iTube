using Microsoft.EntityFrameworkCore;
using VideoService.Application.Interfaces;
using VideoService.Infrastructure.Persistence;

namespace VideoService.Infrastructure.Repositories;

public sealed class UserSubscriptionRepository(VideoDbContext context) : IUserSubscriptionRepository
{
    public async Task<IReadOnlyList<Guid>> GetSubscribedChannelIdsAsync(Guid subscriberId, CancellationToken ct)
        => await context.UserSubscriptions
            .Where(s => s.SubscriberId == subscriberId)
            .Select(s => s.ChannelId)
            .ToListAsync(ct);
}