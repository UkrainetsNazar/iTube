using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public sealed class UserAccountRepository(UserDbContext dbContext) : IUserAccountRepository
{
    public async Task AddAsync(UserAccount userAccount, CancellationToken ct = default) =>
        await dbContext.UserAccounts.AddAsync(userAccount, ct);

    public async Task<bool> ExistsAsync(UserId id, CancellationToken ct = default) =>
        await dbContext.UserAccounts
            .Where(u => u.Id == id).AnyAsync(cancellationToken: ct);

    public async Task<List<UserAccount>> GetAllBannedAsync(CancellationToken ct = default) =>
        await dbContext.UserAccounts
            .Where(u => u.Status == UserAccountStatus.Banned)
            .ToListAsync(cancellationToken: ct);

    public async Task<UserAccount?> GetByIdAsync(UserId id, CancellationToken ct = default) =>
        await dbContext.UserAccounts
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync(cancellationToken: ct);
}