using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace UserService.Domain.Events;
 
public sealed record UserBanned(UserId UserId, UserId ModeratorId, string Reason, DateTime? ExpiresAt) : DomainEvent;