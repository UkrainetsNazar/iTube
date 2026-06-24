namespace AuthService.Application.DTOs;

public sealed record ChangePasswordDto(string OldPassword, string NewPassword);