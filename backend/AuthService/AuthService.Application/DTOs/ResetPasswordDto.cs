namespace AuthService.Application.DTOs;

public sealed record ResetPasswordDto(string Email, string Token, string NewPassword);