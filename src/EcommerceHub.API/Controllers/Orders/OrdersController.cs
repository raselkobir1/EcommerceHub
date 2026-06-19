using EcommerceHub.Modules.Orders.Application.Commands.CancelOrder;
using EcommerceHub.Modules.Orders.Application.Commands.PlaceOrder;
using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Modules.Orders.Application.Queries.GetMyOrders;
using EcommerceHub.Modules.Orders.Application.Queries.GetOrderById;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Orders;

[Route("api/orders")]
public sealed class OrdersController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        if (result.IsSuccess)
            return CreatedAtRoute("GetOrderById", new { id = result.Value.Id },
                ApiResponse<OrderDto>.Ok(result.Value, "Order placed successfully."));
        return BadRequest(ApiResponse<object>.Fail(result.Error!));
    }

    [HttpGet("{id:guid}", Name = "GetOrderById")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new GetOrderByIdQuery(id, currentUser.UserId), ct);
        return HandleResult(result);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized(ApiResponse<object>.Fail("Not authenticated."));

        var result = await Sender.Send(new GetMyOrdersQuery(currentUser.UserId.Value, page, pageSize), ct);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelOrder([FromRoute] Guid id, CancellationToken ct)
    {
        if (!currentUser.UserId.HasValue)
            return Unauthorized(ApiResponse<object>.Fail("Not authenticated."));

        var result = await Sender.Send(new CancelOrderCommand(id, currentUser.UserId.Value), ct);
        if (result.IsSuccess)
            return OkResponse<object>(null!, "Order cancelled successfully.");
        return BadRequest(ApiResponse<object>.Fail(result.Error!));
    }
}
