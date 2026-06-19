using EcommerceHub.Modules.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Auth.Infrastructure.Persistence;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("auth");
        builder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly,
            t => t.Namespace?.Contains("Auth.Infrastructure.Persistence") == true);
        base.OnModelCreating(builder);
    }
}
