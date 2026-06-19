using EcommerceHub.Modules.Orders.Domain.Entities;
using EcommerceHub.Modules.Orders.Domain.Enums;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Orders.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default);
    Task<Order?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountByStatusAsync(OrderStatus status, CancellationToken ct = default);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
