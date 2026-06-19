using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Auth.Infrastructure.Persistence.Repositories;

internal sealed class AdminUserRepository(AuthDbContext context)
    : BaseRepository<AdminUser, AuthDbContext>(context), IAdminUserRepository
{
    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await DbSet
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<AdminUser?> GetByRefreshTokenAsync(string token, CancellationToken ct = default) =>
        await DbSet
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token), ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        await DbSet.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);
}
