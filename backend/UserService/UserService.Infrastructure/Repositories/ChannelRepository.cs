using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public sealed class ChannelRepository(UserDbContext dbContext) : IChannelRepository
{
    public async Task AddAsync(Channel channel, CancellationToken ct = default) =>
        await dbContext.Channels.AddAsync(channel, ct);

    public async Task<Channel?> GetByIdAsync(ChannelId id, CancellationToken ct = default) =>
        await dbContext.Channels
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken: ct);

    public async Task<Channel?> GetByOwnerIdAsync(UserId ownerId, CancellationToken ct = default) =>
        await dbContext.Channels
            .Where(c => c.OwnerId == ownerId)
            .FirstOrDefaultAsync(cancellationToken: ct);
}