using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using Shared.Domain.ValueObjects;

namespace AuthService.Application.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct);
    Task<User?> GetByIdAsync(UserId guid, CancellationToken ct);
    Task<User?> GetByRefreshTokenAsync(string tokenHash, CancellationToken ct);

    Task AddAsync(User user, CancellationToken ct);

    Task<bool> ExistsActiveByEmailAsync(Email email, CancellationToken ct);
}