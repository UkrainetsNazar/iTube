using Shared.Domain.ValueObjects;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface IUserAccountRepository
{
    Task AddAsync(UserAccount userAccount, CancellationToken ct = default);
    Task<bool> ExistsAsync(UserId id, CancellationToken ct = default);
    Task<List<UserAccount>> GetAllBannedAsync(CancellationToken ct = default);
    Task<UserAccount?> GetByIdAsync(UserId id, CancellationToken ct = default);

    Task<(IReadOnlyList<UserAccountAdminRow> Rows, int TotalCount)> GetPagedAsync(
        string? channelNameSearch, int page, int pageSize, CancellationToken ct = default);
}