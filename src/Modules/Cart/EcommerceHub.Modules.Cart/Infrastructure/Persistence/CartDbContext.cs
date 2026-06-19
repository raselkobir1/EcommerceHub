using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;
using CartEntity = EcommerceHub.Modules.Cart.Domain.Entities.Cart;
using CartItemEntity = EcommerceHub.Modules.Cart.Domain.Entities.CartItem;

namespace EcommerceHub.Modules.Cart.Infrastructure.Persistence;

public sealed class CartDbContext(DbContextOptions<CartDbContext> options, AuditInterceptor auditInterceptor)
    : DbContext(options)
{
    public DbSet<CartEntity> Carts => Set<CartEntity>();
    public DbSet<CartItemEntity> CartItems => Set<CartItemEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(auditInterceptor);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cart");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
