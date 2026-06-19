using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Domain.Enums;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Queries.ValidateCoupon;

internal sealed class ValidateCouponQueryHandler(ICouponRepository couponRepository)
    : IRequestHandler<ValidateCouponQuery, Result<ValidateCouponResult>>
{
    public async Task<Result<ValidateCouponResult>> Handle(ValidateCouponQuery request, CancellationToken ct)
    {
        var coupon = await couponRepository.GetByCodeAsync(request.Code.ToUpperInvariant(), ct);

        if (coupon is null)
            return Result.Success(new ValidateCouponResult(false, "Coupon code not found.", 0m, false));

        if (!coupon.IsValid(request.OrderAmount, request.CustomerId))
        {
            var reason = coupon.IsActive switch
            {
                false => "This coupon is no longer active.",
                _ when DateTime.UtcNow < coupon.StartDate => "This coupon is not yet valid.",
                _ when coupon.ExpiryDate.HasValue && DateTime.UtcNow > coupon.ExpiryDate.Value
                    => "This coupon has expired.",
                _ when coupon.MinimumOrderAmount.HasValue && request.OrderAmount < coupon.MinimumOrderAmount.Value
                    => $"Minimum order amount of {coupon.MinimumOrderAmount:F2} required.",
                _ when coupon.TotalUsageLimit.HasValue && coupon.UsedCount >= coupon.TotalUsageLimit.Value
                    => "This coupon has reached its usage limit.",
                _ => "This coupon cannot be used by your account."
            };

            return Result.Success(new ValidateCouponResult(false, reason, 0m, false));
        }

        var discount = coupon.CalculateDiscount(request.OrderAmount);
        var isFreeShipping = coupon.Type == CouponType.FreeShipping;

        return Result.Success(new ValidateCouponResult(true, null, discount, isFreeShipping));
    }
}
