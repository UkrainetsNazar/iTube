using MediatR;
using Shared.Domain.Interfaces;

namespace UserService.Infrastructure.Persistence;

public sealed class UnitOfWork(UserDbContext dbContext, IPublisher publisher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var domainEvents = dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count != 0)
            .SelectMany(aggregate =>
            {
                var events = aggregate.DomainEvents.ToList();
                aggregate.ClearDomainEvents();
                return events;
            })
            .ToList();

        var result = await dbContext.SaveChangesAsync(ct);

        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, ct);

        return result;
    }
}