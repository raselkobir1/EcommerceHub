using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.ToggleCoupon;

internal sealed class ToggleCouponCommandHandler(
    ICouponRepository couponRepository,
    IPromotionsUnitOfWork unitOfWork)
    : IRequestHandler<ToggleCouponCommand, Result<CouponDto>>
{
    public async Task<Result<CouponDto>> Handle(ToggleCouponCommand request, CancellationToken ct)
    {
        var coupon = await couponRepository.GetByIdAsync(request.Id, ct);
        if (coupon is null)
            return Result.Failure<CouponDto>($"Coupon '{request.Id}' not found.");

        coupon.Toggle();
        couponRepository.Update(coupon);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(MapToDto(coupon));
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
