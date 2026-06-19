namespace Shared.Domain.Primitives;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}