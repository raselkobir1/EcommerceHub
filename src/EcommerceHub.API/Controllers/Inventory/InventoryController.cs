using EcommerceHub.Modules.Inventory.Application.Commands.AdjustStock;
using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Modules.Inventory.Application.Queries.GetAdjustments;
using EcommerceHub.Modules.Inventory.Application.Queries.GetInventory;
using EcommerceHub.Modules.Inventory.Application.Queries.GetStockAdjustments;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Inventory;

public sealed record AdjustStockRequest(
    Guid VariantId,
    int QuantityChange,
    string Reason,
    string? Notes);

[Route("api/inventory")]
[Authorize(Policy = "Manager")]
public sealed class InventoryController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
    // -------------------------------------------------------------------------
    // Inventory items (per-variant stock snapshot)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns a paginated snapshot of current stock levels per product/variant.
    /// Optionally filter by name/SKU search term or restrict to low-stock items.
    /// </summary>
    [HttpGet("items")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<InventoryItemDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventory(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? lowStock = null,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetInventoryQuery(search, page, pageSize, lowStock), ct);
        return HandleResult(result);
    }

    // -------------------------------------------------------------------------
    // Stock adjustments (legacy endpoint preserved for backward compatibility)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns a paginated list of stock adjustments. Kept for backward compatibility;
    /// prefer GET /api/inventory/stock-adjustments for the full filtering surface.
    /// </summary>
    [HttpGet("adjustments")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StockAdjustmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdjustments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? productId = null,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetAdjustmentsQuery(page, pageSize, productId), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Returns all stock adjustments for a specific product (up to 100 most recent).
    /// </summary>
    [HttpGet("adjustments/product/{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StockAdjustmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdjustmentsByProduct(
        [FromRoute] Guid productId,
        CancellationToken ct)
    {
        var result = await Sender.Send(new GetAdjustmentsQuery(1, 100, productId), ct);
        return HandleResult(result);
    }

    // -------------------------------------------------------------------------
    // Stock adjustments (new endpoint with extended filtering)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns a paginated list of stock adjustments with optional filtering by
    /// product, variant, and date range.
    /// </summary>
    [HttpGet("stock-adjustments")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StockAdjustmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStockAdjustments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? variantId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(
            new GetStockAdjustmentsQuery(page, pageSize, productId, variantId, from, to), ct);
        return HandleResult(result);
    }

    // -------------------------------------------------------------------------
    // Commands
    // -------------------------------------------------------------------------

    /// <summary>
    /// Records a new stock adjustment (purchase, sale, damage, correction, etc.).
    /// </summary>
    [HttpPost("adjustments")]
    [ProducesResponseType(typeof(ApiResponse<StockAdjustmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAdjustment(
        [FromBody] AdjustStockRequest request,
        CancellationToken ct)
    {
        var adjustedBy = currentUser.UserId?.ToString() ?? "system";
        var command = new AdjustStockCommand(
            request.VariantId,
            request.QuantityChange,
            request.Reason,
            request.Notes,
            adjustedBy);
        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }
}
