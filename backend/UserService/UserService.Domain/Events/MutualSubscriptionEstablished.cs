using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace UserService.Domain.Events;
 
public sealed record MutualSubscriptionEstablished(UserId UserAId, UserId UserBId) : DomainEvent;