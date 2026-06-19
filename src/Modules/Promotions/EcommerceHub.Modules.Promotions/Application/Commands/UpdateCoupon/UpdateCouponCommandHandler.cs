using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Enums;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.UpdateCoupon;

internal sealed class UpdateCouponCommandHandler(
    ICouponRepository couponRepository,
    IPromotionsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCouponCommand, Result<CouponDto>>
{
    public async Task<Result<CouponDto>> Handle(UpdateCouponCommand request, CancellationToken ct)
    {
        var coupon = await couponRepository.GetByIdAsync(request.Id, ct);
        if (coupon is null)
            return Result.Failure<CouponDto>($"Coupon '{request.Id}' not found.");

        if (!Enum.TryParse<CouponType>(request.Type, ignoreCase: true, out var couponType))
            return Result.Failure<CouponDto>($"Invalid coupon type '{request.Type}'.");

        var codeChanged = !string.Equals(coupon.Code, request.Code.ToUpperInvariant(), StringComparison.Ordinal);
        if (codeChanged && await couponRepository.CodeExistsAsync(request.Code, excludeId: request.Id, ct))
            return Result.Failure<CouponDto>($"A coupon with code '{request.Code.ToUpperInvariant()}' already exists.");

        coupon.Update(
            request.Code,
            couponType,
            request.Value,
            request.StartDate,
            request.ExpiryDate,
            request.MinimumOrderAmount,
            request.TotalUsageLimit,
            request.PerCustomerLimit);

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
