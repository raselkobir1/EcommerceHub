using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Commands.ToggleCoupon;

public sealed record ToggleCouponCommand(Guid Id) : IRequest<Result<CouponDto>>;
