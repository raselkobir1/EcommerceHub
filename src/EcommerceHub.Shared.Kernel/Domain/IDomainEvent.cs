using MediatR;

namespace EcommerceHub.Shared.Kernel.Domain;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
