using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace UserService.Domain.Events;
 
public sealed record ChannelUnbanned(ChannelId ChannelId) : DomainEvent;