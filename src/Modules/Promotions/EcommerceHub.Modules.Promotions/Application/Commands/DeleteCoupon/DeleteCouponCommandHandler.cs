using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.DeleteCoupon;

internal sealed class DeleteCouponCommandHandler(
    ICouponRepository couponRepository,
    IPromotionsUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCouponCommand, Result>
{
    public async Task<Result> Handle(DeleteCouponCommand request, CancellationToken ct)
    {
        var coupon = await couponRepository.GetByIdAsync(request.Id, ct);
        if (coupon is null)
            return Result.Failure($"Coupon '{request.Id}' not found.");

        // Soft delete: deactivate so existing usage records are preserved.
        coupon.Deactivate();
        couponRepository.Update(coupon);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
