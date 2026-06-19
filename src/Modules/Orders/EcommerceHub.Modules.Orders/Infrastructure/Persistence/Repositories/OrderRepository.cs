using EcommerceHub.Modules.Orders.Domain.Entities;
using EcommerceHub.Modules.Orders.Domain.Enums;
using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Orders.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(OrdersDbContext context)
    : BaseRepository<Order, OrdersDbContext>(context), IOrderRepository
{
    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default)
        => await context.Orders.Include(o => o.Items).Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, ct);

    public async Task<Order?> GetWithDetailsAsync(Guid id, CancellationToken ct = default)
        => await context.Orders.Include(o => o.Items).Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await context.Orders.Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, int page, int pageSize, CancellationToken ct = default)
        => await context.Orders.Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> CountByStatusAsync(OrderStatus status, CancellationToken ct = default)
        => await context.Orders.CountAsync(o => o.Status == status, ct);

    public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await context.Orders
            .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt >= from && o.CreatedAt <= to)
            .SumAsync(o => o.GrandTotal, ct);
}
