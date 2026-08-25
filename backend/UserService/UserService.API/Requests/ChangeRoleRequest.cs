using UserService.Domain.Enums;

namespace UserService.API.Requests;

public sealed record ChangeRoleRequest(UserRole Role);