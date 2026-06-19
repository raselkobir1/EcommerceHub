using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Promotions.Application.Queries.GetCoupons;

public sealed record GetCouponsQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<CouponDto>>>;
