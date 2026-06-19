using EcommerceHub.Modules.Suppliers.Domain.Entities;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Suppliers.Infrastructure.Persistence;

public sealed class SuppliersDbContext(DbContextOptions<SuppliersDbContext> options, AuditInterceptor auditInterceptor)
    : DbContext(options)
{
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(auditInterceptor);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("suppliers");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SuppliersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
