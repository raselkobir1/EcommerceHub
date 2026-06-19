using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Promotions.Infrastructure.Persistence.Repositories;

internal sealed class CouponRepository(PromotionsDbContext context)
    : BaseRepository<Coupon, PromotionsDbContext>(context), ICouponRepository
{
    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await context.Coupons.Include(c => c.ScopeItems).Include(c => c.Usages)
            .FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant(), ct);

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken ct = default)
        => await context.Coupons.AnyAsync(c => c.Code == code.ToUpperInvariant() &&
                                               (excludeId == null || c.Id != excludeId), ct);
}
