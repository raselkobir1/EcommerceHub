using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.CreateCoupon;

public sealed record CreateCouponCommand(
    string Code,
    string Type,
    decimal Value,
    DateTime StartDate,
    DateTime? ExpiryDate,
    decimal? MinimumOrderAmount,
    int? TotalUsageLimit,
    int? PerCustomerLimit) : IRequest<Result<CouponDto>>;
