using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Promotions.Domain.Interfaces;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken ct = default);
}

public interface IFlashSaleRepository : IRepository<FlashSale>
{
    Task<IEnumerable<FlashSale>> GetActiveAsync(CancellationToken ct = default);
}

public interface IBannerRepository : IRepository<Banner>
{
    Task<IEnumerable<Banner>> GetActiveAsync(string? placement = null, CancellationToken ct = default);
}
