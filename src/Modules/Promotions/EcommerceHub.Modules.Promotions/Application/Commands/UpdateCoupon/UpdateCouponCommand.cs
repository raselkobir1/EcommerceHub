using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.UpdateCoupon;

public sealed record UpdateCouponCommand(
    Guid Id,
    string Code,
    string Type,
    decimal Value,
    DateTime StartDate,
    DateTime? ExpiryDate,
    decimal? MinimumOrderAmount,
    int? TotalUsageLimit,
    int? PerCustomerLimit) : IRequest<Result<CouponDto>>;
