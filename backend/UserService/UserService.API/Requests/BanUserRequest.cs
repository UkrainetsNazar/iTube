namespace UserService.API.Requests;

public sealed record BanUserRequest(string Reason, DateTime? ExpiresAt);