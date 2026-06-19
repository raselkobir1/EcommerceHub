using EcommerceHub.Modules.Reviews.Domain.Entities;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Reviews.Infrastructure.Persistence;

public sealed class ReviewsDbContext(DbContextOptions<ReviewsDbContext> options, AuditInterceptor auditInterceptor)
    : DbContext(options)
{
    public DbSet<ProductReview> ProductReviews => Set<ProductReview>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(auditInterceptor);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reviews");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReviewsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
