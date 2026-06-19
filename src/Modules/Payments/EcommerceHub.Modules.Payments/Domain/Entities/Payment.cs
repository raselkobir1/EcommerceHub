using EcommerceHub.Modules.Payments.Domain.Enums;
using EcommerceHub.Modules.Payments.Domain.Events;
using EcommerceHub.Shared.Kernel.Domain;
using EcommerceHub.Shared.Kernel.Exceptions;

namespace EcommerceHub.Modules.Payments.Domain.Entities;

public sealed class Payment : AuditableEntity
{
    public Guid OrderId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "BDT";
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? GatewayReference { get; private set; }
    public string? GatewayResponse { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    public decimal RefundedAmount { get; private set; }
    public string? RefundReason { get; private set; }
    public string? CustomerPhone { get; private set; }
    public string? CustomerEmail { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;

    private readonly List<PaymentRefund> _refunds = [];
    public IReadOnlyCollection<PaymentRefund> Refunds => _refunds.AsReadOnly();

    private Payment() { }

    public static Payment Create(
        Guid orderId,
        string orderNumber,
        Guid customerId,
        decimal amount,
        PaymentMethod method,
        string ipAddress,
        string? customerPhone = null,
        string? customerEmail = null)
    {
        if (amount <= 0) throw new DomainException("Payment amount must be positive.");

        var payment = new Payment
        {
            OrderId = orderId,
            OrderNumber = orderNumber,
            CustomerId = customerId,
            Amount = amount,
            Method = method,
            Status = PaymentStatus.Pending,
            IpAddress = ipAddress,
            CustomerPhone = customerPhone,
            CustomerEmail = customerEmail
        };
        payment.RaiseDomainEvent(new PaymentInitiatedEvent(payment.Id, orderId, orderNumber, amount, method.ToString()));
        return payment;
    }

    public void MarkSuccess(string gatewayTransactionId, string? gatewayReference = null, string? gatewayResponse = null)
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Processing)
            throw new DomainException($"Cannot mark payment as successful from status {Status}.");

        Status = PaymentStatus.Completed;
        GatewayTransactionId = gatewayTransactionId;
        GatewayReference = gatewayReference;
        GatewayResponse = gatewayResponse;
        PaidAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentCompletedEvent(Id, OrderId, OrderNumber, Amount, CustomerId, gatewayTransactionId));
    }

    public void MarkFailed(string reason, string? gatewayResponse = null)
    {
        if (Status == PaymentStatus.Completed || Status == PaymentStatus.Refunded)
            throw new DomainException($"Cannot mark payment as failed from status {Status}.");

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        GatewayResponse = gatewayResponse;
        RaiseDomainEvent(new PaymentFailedEvent(Id, OrderId, OrderNumber, Amount, reason));
    }

    public void MarkProcessing(string? gatewayReference = null)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be set to processing.");

        Status = PaymentStatus.Processing;
        GatewayReference = gatewayReference;
    }

    public PaymentRefund ProcessRefund(decimal refundAmount, string reason, string initiatedBy)
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException("Only completed payments can be refunded.");

        var totalRefunded = RefundedAmount + refundAmount;
        if (totalRefunded > Amount)
            throw new DomainException($"Refund amount ({totalRefunded:F2} BDT) exceeds payment amount ({Amount:F2} BDT).");

        var refund = PaymentRefund.Create(Id, refundAmount, reason, initiatedBy);
        _refunds.Add(refund);
        RefundedAmount = totalRefunded;
        RefundReason = reason;

        if (RefundedAmount >= Amount)
        {
            Status = PaymentStatus.Refunded;
            RefundedAt = DateTime.UtcNow;
        }
        else
        {
            Status = PaymentStatus.PartiallyRefunded;
        }

        RaiseDomainEvent(new PaymentRefundedEvent(Id, OrderId, OrderNumber, refundAmount, RefundedAmount, reason));
        return refund;
    }
}
