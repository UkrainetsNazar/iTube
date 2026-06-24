namespace AuthService.Application.DTOs;

public sealed record ConfirmEmailDto(string Email, string Token);