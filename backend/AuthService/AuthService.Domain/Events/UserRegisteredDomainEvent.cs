using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public sealed record UserRegisteredDomainEvent(
    UserId UserId,
    string Email,
    string ConfirmationToken) : DomainEvent;