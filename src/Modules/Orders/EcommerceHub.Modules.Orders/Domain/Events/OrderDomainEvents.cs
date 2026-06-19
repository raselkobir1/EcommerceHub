using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Orders.Domain.Events;

public sealed record OrderPlacedEvent(Guid OrderId, string OrderNumber, string CustomerEmail, decimal GrandTotal) : DomainEvent;
public sealed record OrderConfirmedEvent(Guid OrderId, string OrderNumber, string CustomerEmail) : DomainEvent;
public sealed record OrderShippedEvent(Guid OrderId, string OrderNumber, string CustomerEmail, string TrackingNumber) : DomainEvent;
public sealed record OrderDeliveredEvent(Guid OrderId, string OrderNumber, string CustomerEmail) : DomainEvent;
public sealed record OrderCancelledEvent(Guid OrderId, string OrderNumber, string CustomerEmail, string Reason) : DomainEvent;
