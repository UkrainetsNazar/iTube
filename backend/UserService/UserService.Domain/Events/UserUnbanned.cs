using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace UserService.Domain.Events;
 
public sealed record UserUnbanned(UserId UserId, UserId? UnbannedByModeratorId) : DomainEvent;