using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Promotions.Infrastructure.Persistence.Repositories;

internal sealed class FlashSaleRepository(PromotionsDbContext context)
    : BaseRepository<FlashSale, PromotionsDbContext>(context), IFlashSaleRepository
{
    public async Task<IEnumerable<FlashSale>> GetActiveAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        return await context.FlashSales
            .Where(f => f.IsActive && f.StartAt <= now && f.EndAt >= now)
            .ToListAsync(ct);
    }
}
