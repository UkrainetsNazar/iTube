using Shared.Domain.ValueObjects;
using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface IUserAccountRepository
{
    Task<UserAccount?> GetByIdAsync(UserId id, CancellationToken ct = default);
    Task<bool> ExistsAsync(UserId id, CancellationToken ct = default);
    Task AddAsync(UserAccount userAccount, CancellationToken ct = default);
    Task<List<UserAccount>> GetAllBannedAsync(CancellationToken ct = default);
}