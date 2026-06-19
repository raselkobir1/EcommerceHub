using EcommerceHub.Shared.Kernel.Domain;

namespace EcommerceHub.Modules.Payments.Domain.Entities;

public sealed class PaymentRefund : BaseEntity
{
    public Guid PaymentId { get; private set; }
    public decimal Amount { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string InitiatedBy { get; private set; } = string.Empty;
    public DateTime RefundedAt { get; private set; }
    public string? GatewayRefundId { get; private set; }

    private PaymentRefund() { }

    public static PaymentRefund Create(Guid paymentId, decimal amount, string reason, string initiatedBy)
        => new()
        {
            PaymentId = paymentId,
            Amount = amount,
            Reason = reason,
            InitiatedBy = initiatedBy,
            RefundedAt = DateTime.UtcNow
        };

    public void SetGatewayRefundId(string refundId) => GatewayRefundId = refundId;
}
