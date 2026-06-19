using EcommerceHub.Modules.Orders.Application.Commands.CancelOrder;
using EcommerceHub.Modules.Orders.Application.Commands.PlaceOrder;
using EcommerceHub.Modules.Orders.Application.Commands.UpdateOrderStatus;
using EcommerceHub.Modules.Orders.Application.DTOs;
using EcommerceHub.Modules.Orders.Application.Queries.GetMyOrders;
using EcommerceHub.Modules.Orders.Application.Queries.GetOrderById;
using EcommerceHub.Modules.Orders.Application.Queries.GetOrders;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Orders;

[Route("api/orders")]
public sealed class OrdersController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
    // ── Customer: place order ────────────────────────────────────────────────

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

    // ── Admin: list all orders ───────────────────────────────────────────────

    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetOrdersQuery(status, page, pageSize), ct);
        return HandleResult(result);
    }

    // ── Shared: get single order ─────────────────────────────────────────────

    [HttpGet("{id:guid}", Name = "GetOrderById")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid id, CancellationToken ct)
    {
        // Admins pass null requester so the handler skips the ownership check.
        var requesterId = User.IsInRole("Admin") || User.IsInRole("SuperAdmin")
            ? (Guid?)null
            : currentUser.UserId;

        var result = await Sender.Send(new GetOrderByIdQuery(id, requesterId), ct);
        return HandleResult(result);
    }

    // ── Customer: my orders ──────────────────────────────────────────────────

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

    // ── Admin: update order status ───────────────────────────────────────────

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateOrderStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateOrderStatusRequest body,
        CancellationToken ct)
    {
        var updatedBy = currentUser.UserId?.ToString() ?? "admin";
        var result = await Sender.Send(
            new UpdateOrderStatusCommand(id, body.Status, body.Notes, updatedBy), ct);

        if (result.IsSuccess)
            return OkResponse<object>(null!, "Order status updated successfully.");
        return BadRequest(ApiResponse<object>.Fail(result.Error!));
    }

    // ── Customer: cancel order ───────────────────────────────────────────────

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

/// <summary>Request body for PUT /api/orders/{id}/status</summary>
public sealed record UpdateOrderStatusRequest(string Status, string? Notes);
