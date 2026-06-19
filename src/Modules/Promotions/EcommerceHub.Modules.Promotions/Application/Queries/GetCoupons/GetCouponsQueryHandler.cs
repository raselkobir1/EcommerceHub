using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Queries.GetCoupons;

internal sealed class GetCouponsQueryHandler(ICouponRepository couponRepository)
    : IRequestHandler<GetCouponsQuery, Result<PagedResult<CouponDto>>>
{
    public async Task<Result<PagedResult<CouponDto>>> Handle(GetCouponsQuery request, CancellationToken ct)
    {
        var all = await couponRepository.GetAllAsync(ct);

        var totalCount = all.Count;
        var items = all
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(MapToDto)
            .ToList()
            .AsReadOnly();

        var paged = PagedResult<CouponDto>.Create(items, totalCount, request.Page, request.PageSize);
        return Result.Success(paged);
    }

    private static CouponDto MapToDto(Coupon c) => new(
        c.Id,
        c.Code,
        c.Type.ToString(),
        c.Value,
        c.MinimumOrderAmount,
        c.TotalUsageLimit,
        c.PerCustomerLimit,
        c.UsedCount,
        c.StartDate,
        c.ExpiryDate,
        c.IsActive);
}
