using UserService.Domain.Enums;
using Shared.Domain.Primitives;
using Shared.Domain.ValueObjects;


namespace UserService.Domain.Events;
 
public sealed record RoleChanged(UserId UserId, UserRole NewRole, UserId ChangedBy) : DomainEvent;