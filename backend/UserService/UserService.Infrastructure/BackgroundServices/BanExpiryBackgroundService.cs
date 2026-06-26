using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Domain.Interfaces;
using UserService.Application.Interfaces;

namespace UserService.Infrastructure.BackgroundServices;

public sealed class BanExpiryBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<BanExpiryBackgroundService> logger
) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(Interval, ct);

            try
            {
                await ProcessExpiredBansAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing expired bans.");
            }
        }
    }

    private async Task ProcessExpiredBansAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var userAccountRepository = scope.ServiceProvider.GetRequiredService<IUserAccountRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var bannedUsers = await userAccountRepository.GetAllBannedAsync(ct);

        var expiredCount = 0;
        foreach (var user in bannedUsers)
        {
            user.ExpireBanIfNeeded();

            if (user.Status == Domain.Enums.UserAccountStatus.Active)
                expiredCount++;
        }

        if (expiredCount > 0)
        {
            await unitOfWork.SaveChangesAsync(ct);
            logger.LogInformation("Automatically unbanned {Count} users with expired bans.", expiredCount);
        }
    }
}