using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Interfaces;

namespace MediaService.Infrastructure.Persistence;

public sealed class UnitOfWork(MediaDbContext dbContext, IPublisher publisher) : IUnitOfWork
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

        foreach (var domainEvent in domainEvents)
            await publisher.Publish(domainEvent, ct);

        try
        {
            return await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                Console.WriteLine($"Concurrency conflict: {entry.Entity.GetType().Name}, State: {entry.State}");
            }
            throw;
        }
    }
}