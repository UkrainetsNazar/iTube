using Shared.Domain.ValueObjects;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface IChannelRepository
{
    Task<Channel?> GetByIdAsync(ChannelId id, CancellationToken ct = default);
    Task<Channel?> GetByOwnerIdAsync(UserId ownerId, CancellationToken ct = default);
    Task AddAsync(Channel channel, CancellationToken ct = default);
}