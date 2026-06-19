using EcommerceHub.Modules.Promotions.Application.Commands.CreateCoupon;
using EcommerceHub.Modules.Promotions.Application.DTOs;
using EcommerceHub.Modules.Promotions.Application.Queries.GetCoupons;
using EcommerceHub.Modules.Promotions.Application.Queries.ValidateCoupon;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Promotions;

public sealed record ValidateCouponRequest(string Code, decimal OrderAmount);

[Route("api/coupons")]
public sealed class CouponsController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
    [HttpGet]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CouponDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCoupons(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetCouponsQuery(page, pageSize), ct);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<CouponDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }

    [HttpPost("validate")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ValidateCouponResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponRequest request, CancellationToken ct)
    {
        var query = new ValidateCouponQuery(request.Code, request.OrderAmount, currentUser.UserId);
        var result = await Sender.Send(query, ct);
        return HandleResult(result);
    }
}
