using EcommerceHub.Modules.Inventory.Application.Commands.AdjustStock;
using EcommerceHub.Modules.Inventory.Application.DTOs;
using EcommerceHub.Modules.Inventory.Application.Queries.GetAdjustments;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Inventory;

public sealed record AdjustStockRequest(
    Guid ProductId, Guid VariantId,
    string ProductName, string Sku,
    int QuantityChange, int CurrentStock,
    string Reason, string? Notes);

[Route("api/inventory")]
[Authorize(Policy = "Manager")]
public sealed class InventoryController(MediatR.ISender sender, ICurrentUser currentUser) : ApiController(sender)
{
    [HttpGet("adjustments")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StockAdjustmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdjustments(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] Guid? productId = null, CancellationToken ct = default)
    {
        var result = await Sender.Send(new GetAdjustmentsQuery(page, pageSize, productId), ct);
        return HandleResult(result);
    }

    [HttpGet("adjustments/product/{productId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<StockAdjustmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdjustmentsByProduct([FromRoute] Guid productId, CancellationToken ct)
    {
        var result = await Sender.Send(new GetAdjustmentsQuery(1, 100, productId), ct);
        return HandleResult(result);
    }

    [HttpPost("adjustments")]
    [ProducesResponseType(typeof(ApiResponse<StockAdjustmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAdjustment([FromBody] AdjustStockRequest request, CancellationToken ct)
    {
        var adjustedBy = currentUser.UserId?.ToString() ?? "system";
        var command = new AdjustStockCommand(
            request.ProductId, request.VariantId,
            request.ProductName, request.Sku,
            request.QuantityChange, request.CurrentStock,
            request.Reason, request.Notes, adjustedBy);
        var result = await Sender.Send(command, ct);
        return HandleResult(result);
    }
}
