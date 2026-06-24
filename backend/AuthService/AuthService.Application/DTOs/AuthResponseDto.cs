namespace AuthService.Application.DTOs;

public sealed record AuthResponseDto(string AccessToken, string RefreshToken);