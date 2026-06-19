using EcommerceHub.Modules.Orders.Domain.Enums;
using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Orders.Domain.Entities;

public sealed class OrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string Note { get; private set; } = default!;
    public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;
    public DateTime ChangedAt { get; private set; } = DateTime.UtcNow;
    public string? ChangedBy { get; private set; }

    private OrderStatusHistory() { }

    public static OrderStatusHistory Create(Guid orderId, OrderStatus status, string note, string? changedBy = null) =>
        new() { OrderId = orderId, Status = status, Note = note, ChangedAt = DateTime.UtcNow, ChangedBy = changedBy };
}
