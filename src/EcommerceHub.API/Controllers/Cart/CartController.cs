using EcommerceHub.Modules.Cart.Application.Commands.AddToCart;
using EcommerceHub.Modules.Cart.Application.Commands.RemoveCartItem;
using EcommerceHub.Modules.Cart.Application.Commands.UpdateCartItem;
using EcommerceHub.Modules.Cart.Application.DTOs;
using EcommerceHub.Modules.Cart.Application.Queries.GetCart;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Cart;

public sealed record AddToCartRequest(
    Guid ProductId, Guid VariantId,
    string ProductName, string Sku, string? ImageUrl,
    decimal UnitPrice, int Quantity = 1);

public sealed record UpdateCartItemRequest(int Quantity);

[Route("api/cart")]
[Authorize]
public sealed class CartController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
    private string? SessionId => HttpContext.Request.Headers["X-Session-Id"].FirstOrDefault();

    /// <summary>Get the current user's active cart.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var query = new GetCartQuery(currentUser.UserId, SessionId);
        var result = await Sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>Add a product variant to the cart.</summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request, CancellationToken ct)
    {
        var command = new AddToCartCommand(
            currentUser.UserId, SessionId,
            request.ProductId, request.VariantId,
            request.ProductName, request.Sku, request.ImageUrl,
            request.UnitPrice, request.Quantity);
        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Update the quantity of a specific variant in the cart.</summary>
    [HttpPut("items/{variantId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateItem([FromRoute] Guid variantId, [FromBody] UpdateCartItemRequest request, CancellationToken ct)
    {
        var command = new UpdateCartItemCommand(currentUser.UserId, SessionId, variantId, request.Quantity);
        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>Remove a specific variant from the cart.</summary>
    [HttpDelete("items/{variantId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveItem([FromRoute] Guid variantId, CancellationToken ct)
    {
        var command = new RemoveCartItemCommand(currentUser.UserId, SessionId, variantId);
        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Merge a guest cart into the authenticated customer's persistent cart.
    /// Call this after a guest logs in or registers.
    /// </summary>
    [HttpPost("merge")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult MergeCart([FromBody] object request)
        => NotImplementedResponse();
}
