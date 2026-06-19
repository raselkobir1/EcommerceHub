using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Payments.Domain.Events;

public sealed record PaymentInitiatedEvent(
    Guid PaymentId, Guid OrderId, string OrderNumber, decimal Amount, string Method) : DomainEvent;

public sealed record PaymentCompletedEvent(
    Guid PaymentId, Guid OrderId, string OrderNumber, decimal Amount, Guid CustomerId, string GatewayTransactionId) : DomainEvent;

public sealed record PaymentFailedEvent(
    Guid PaymentId, Guid OrderId, string OrderNumber, decimal Amount, string Reason) : DomainEvent;

public sealed record PaymentRefundedEvent(
    Guid PaymentId, Guid OrderId, string OrderNumber,
    decimal RefundAmount, decimal TotalRefunded, string Reason) : DomainEvent;
