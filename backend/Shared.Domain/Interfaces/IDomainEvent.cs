using MediatR;

namespace Shared.Domain.Interfaces;

public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}