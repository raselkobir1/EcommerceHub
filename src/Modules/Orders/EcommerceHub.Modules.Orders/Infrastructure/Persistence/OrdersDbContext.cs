using EcommerceHub.Modules.Orders.Domain.Entities;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Orders.Infrastructure.Persistence;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options, AuditInterceptor auditInterceptor) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(auditInterceptor);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("orders");
        builder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly,
            t => t.Namespace?.Contains("Orders.Infrastructure.Persistence") == true);
        base.OnModelCreating(builder);
    }
}
