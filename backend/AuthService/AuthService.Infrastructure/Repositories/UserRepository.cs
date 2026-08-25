using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;

namespace AuthService.Infrastructure.Repositories;

public sealed class UserRepository(AuthDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct) =>
        await dbContext.Users.AddAsync(user, ct);

    public async Task<bool> ExistsActiveByEmailAsync(Email email, CancellationToken ct) =>
        await dbContext.Users
            .AnyAsync(u => u.Email == email && u.Status != AccountStatus.PendingConfirmation, ct);

    public async Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken ct) =>
        await dbContext.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct) =>
        await dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct) =>
        await dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByRefreshTokenAsync(string tokenHash, CancellationToken ct)
        => await dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens
                .Any(rt => rt.TokenHash == tokenHash), ct);
}