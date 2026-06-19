using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Promotions.Infrastructure.Persistence.Repositories;

internal sealed class BannerRepository(PromotionsDbContext context)
    : BaseRepository<Banner, PromotionsDbContext>(context), IBannerRepository
{
    public async Task<IEnumerable<Banner>> GetActiveAsync(string? placement = null, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var query = context.Banners
            .Where(b => b.IsActive &&
                        (!b.ActiveFrom.HasValue || b.ActiveFrom <= now) &&
                        (!b.ActiveTo.HasValue || b.ActiveTo >= now));

        // Banner entity does not carry a Placement property; placement filtering is a no-op at this layer.
        _ = placement;

        return await query.OrderBy(b => b.SortOrder).ToListAsync(ct);
    }
}
