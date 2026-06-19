using EcommerceHub.Shared.Kernel.Abstractions;
using CartEntity = EcommerceHub.Modules.Cart.Domain.Entities.Cart;

namespace EcommerceHub.Modules.Cart.Domain.Interfaces;

public interface ICartRepository : IRepository<CartEntity>
{
    Task<CartEntity?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<CartEntity?> GetBySessionIdAsync(string sessionId, CancellationToken ct = default);
    Task<CartEntity?> GetWithItemsAsync(Guid cartId, CancellationToken ct = default);
}
