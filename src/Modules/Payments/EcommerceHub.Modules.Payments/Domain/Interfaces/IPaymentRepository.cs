using EcommerceHub.Modules.Payments.Domain.Entities;
using EcommerceHub.Modules.Payments.Domain.Enums;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Payments.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<Payment?> GetByGatewayTransactionIdAsync(string transactionId, CancellationToken ct = default);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken ct = default);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
