using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;
using UserService.Domain.Enums;

namespace UserService.Domain.Events;
 
public sealed record RoleChanged(UserId UserId, UserRole NewRole, UserId ChangedBy) : DomainEvent;