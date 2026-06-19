using EcommerceHub.Modules.Payments.Domain.Entities;
using EcommerceHub.Modules.Payments.Domain.Enums;
using EcommerceHub.Modules.Payments.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Payments.Infrastructure.Persistence.Repositories;

internal sealed class PaymentRepository(PaymentsDbContext context)
    : BaseRepository<Payment, PaymentsDbContext>(context), IPaymentRepository
{
    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        => await context.Payments.Include(p => p.Refunds)
            .FirstOrDefaultAsync(p => p.OrderId == orderId, ct);

    public async Task<IEnumerable<Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await context.Payments.Include(p => p.Refunds)
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task<Payment?> GetByGatewayTransactionIdAsync(string transactionId, CancellationToken ct = default)
        => await context.Payments.FirstOrDefaultAsync(p => p.GatewayTransactionId == transactionId, ct);

    public async Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken ct = default)
        => await context.Payments.Where(p => p.Status == status).ToListAsync(ct);

    public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await context.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.PaidAt >= from && p.PaidAt <= to)
            .SumAsync(p => p.Amount - p.RefundedAmount, ct);
}
