using EcommerceHub.Modules.Cart.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;
using CartEntity = EcommerceHub.Modules.Cart.Domain.Entities.Cart;

namespace EcommerceHub.Modules.Cart.Infrastructure.Persistence.Repositories;

internal sealed class CartRepository(CartDbContext context)
    : BaseRepository<CartEntity, CartDbContext>(context), ICartRepository
{
    public async Task<CartEntity?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await context.Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, ct);

    public async Task<CartEntity?> GetBySessionIdAsync(string sessionId, CancellationToken ct = default)
        => await context.Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId, ct);

    public async Task<CartEntity?> GetWithItemsAsync(Guid cartId, CancellationToken ct = default)
        => await context.Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId, ct);
}
