using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;

namespace AuthService.Domain.Events;

public sealed record UserRegistered(UserId UserId, string Email) : DomainEvent;