using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace UserService.Domain.Events;
 
public sealed record ChannelBanned(ChannelId ChannelId, string Reason) : DomainEvent;