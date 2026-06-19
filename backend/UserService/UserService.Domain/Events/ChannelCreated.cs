using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace UserService.Domain.Events;
 
public sealed record ChannelCreated(ChannelId ChannelId, UserId OwnerId) : DomainEvent;