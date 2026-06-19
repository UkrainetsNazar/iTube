using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace UserService.Domain.Events;
 
public sealed record UserUnsubscribed(UserId SubscriberId, ChannelId TargetChannelId) : DomainEvent;