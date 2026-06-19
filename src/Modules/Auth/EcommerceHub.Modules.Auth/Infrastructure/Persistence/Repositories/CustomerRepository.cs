using EcommerceHub.Modules.Auth.Domain.Entities;
using EcommerceHub.Modules.Auth.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Auth.Infrastructure.Persistence.Repositories;

internal sealed class CustomerRepository(AuthDbContext context)
    : BaseRepository<Customer, AuthDbContext>(context), ICustomerRepository
{
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await DbSet
            .Include(c => c.RefreshTokens)
            .FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant(), ct);

    public async Task<Customer?> GetByRefreshTokenAsync(string token, CancellationToken ct = default) =>
        await DbSet
            .Include(c => c.RefreshTokens)
            .FirstOrDefaultAsync(c => c.RefreshTokens.Any(t => t.Token == token), ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        await DbSet.AnyAsync(c => c.Email == email.ToLowerInvariant(), ct);

    public async Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default) =>
        await DbSet.AnyAsync(c => c.Phone == phone, ct);
}
