using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Domain.Entities;
using EcommerceHub.Modules.Promotions.Domain.Enums;
using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.CreateCoupon;

internal sealed class CreateCouponCommandHandler(
    ICouponRepository couponRepository,
    IPromotionsUnitOfWork unitOfWork)
    : IRequestHandler<CreateCouponCommand, Result<CouponDto>>
{
    public async Task<Result<CouponDto>> Handle(CreateCouponCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<CouponType>(request.Type, ignoreCase: true, out var couponType))
            return Result.Failure<CouponDto>($"Invalid coupon type '{request.Type}'.");

        if (await couponRepository.CodeExistsAsync(request.Code, excludeId: null, ct))
            return Result.Failure<CouponDto>($"A coupon with code '{request.Code.ToUpperInvariant()}' already exists.");

        var coupon = Coupon.Create(
            request.Code,
            couponType,
            request.Value,
            request.StartDate,
            request.ExpiryDate,
            request.MinimumOrderAmount,
            request.TotalUsageLimit,
            request.PerCustomerLimit);

        await couponRepository.AddAsync(coupon, ct);
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
