using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public sealed record PasswordResetRequestedDomainEvent(
    UserId UserId,
    string Email,
    string RawToken) : DomainEvent;