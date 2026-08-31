using Microsoft.EntityFrameworkCore;
using Shared.Domain.ValueObjects;
using UserService.Application.DTOs;
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

    public async Task<(IReadOnlyList<UserAccountAdminRow>, int)> GetPagedAsync(
        string? channelNameSearch, int page, int pageSize, CancellationToken ct = default)
    {
        var query =
            from account in dbContext.UserAccounts
            join channel in dbContext.Channels on account.Id.Value equals channel.Id.Value
            select new { account, channel };

        if (!string.IsNullOrWhiteSpace(channelNameSearch))
            query = query.Where(x => EF.Functions.ILike(x.channel.Name.Value, $"%{channelNameSearch}%"));

        var total = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(x => x.channel.Name.Value)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new UserAccountAdminRow(
                x.account.Id.Value, x.channel.Name.Value,
                x.account.Role.ToString(), x.account.Status.ToString(),
                x.account.CurrentBan != null,
                x.account.CurrentBan != null ? x.account.CurrentBan.Reason : null,
                x.account.CurrentBan != null ? x.account.CurrentBan.ExpiresAt : null,
                x.channel.SubscribersCount, x.account.CreatedAt))
            .ToListAsync(ct);

        return (rows, total);
    }
}