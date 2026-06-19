using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Queries.ValidateCoupon;

public sealed record ValidateCouponQuery(string Code, decimal OrderAmount, Guid? CustomerId)
    : IRequest<Result<ValidateCouponResult>>;
