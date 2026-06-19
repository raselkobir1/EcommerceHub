using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Promotions.Infrastructure.Persistence;

public sealed class PromotionsDbContext(DbContextOptions<PromotionsDbContext> options, AuditInterceptor auditInterceptor)
    : DbContext(options)
{
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<FlashSale> FlashSales => Set<FlashSale>();
    public DbSet<Banner> Banners => Set<Banner>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(auditInterceptor);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("promotions");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PromotionsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
