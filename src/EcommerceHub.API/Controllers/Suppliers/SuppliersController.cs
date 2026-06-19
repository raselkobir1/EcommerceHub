using EcommerceHub.Modules.Suppliers.Application.Commands.CreateSupplier;
using EcommerceHub.Modules.Suppliers.Application.Commands.DeleteSupplier;
using EcommerceHub.Modules.Suppliers.Application.Commands.UpdateSupplier;
using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Modules.Suppliers.Application.Queries.GetSupplierById;
using EcommerceHub.Modules.Suppliers.Application.Queries.GetSuppliers;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Suppliers;

[Route("api/suppliers")]
[Authorize(Policy = "Manager")]
public sealed class SuppliersController(MediatR.ISender sender) : ApiController(sender)
{
    /// <summary>
    /// Get a paginated list of all suppliers.
    /// Supports optional free-text search across company name, contact person, email and phone.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SupplierListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuppliers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
    {
        var query = new GetSuppliersQuery(page, pageSize, searchTerm, isActive);
        var result = await Sender.Send(query, ct);

        if (result.IsSuccess)
            return Ok(ApiResponse<PagedResult<SupplierListDto>>.Ok(result.Value));

        return BadRequest(ApiResponse<PagedResult<SupplierListDto>>.Fail(result.Error!));
    }

    /// <summary>Get full details of a single supplier by ID, including financial summary.</summary>
    [HttpGet("{id:guid}", Name = "GetSupplierById")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupplierById([FromRoute] Guid id, CancellationToken ct = default)
    {
        var query = new GetSupplierByIdQuery(id);
        var result = await Sender.Send(query, ct);

        if (result.IsSuccess)
            return Ok(ApiResponse<SupplierDto>.Ok(result.Value));

        return NotFound(ApiResponse<SupplierDto>.Fail(result.Error!));
    }

    /// <summary>Create a new supplier record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSupplier(
        [FromBody] CreateSupplierCommand command,
        CancellationToken ct = default)
    {
        var result = await Sender.Send(command, ct);

        if (result.IsSuccess)
            return CreatedAtRoute(
                "GetSupplierById",
                new { id = result.Value.Id },
                ApiResponse<SupplierDto>.Ok(result.Value, "Supplier created successfully."));

        return BadRequest(ApiResponse<SupplierDto>.Fail(result.Error!));
    }

    /// <summary>Update an existing supplier's details.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSupplier(
        [FromRoute] Guid id,
        [FromBody] UpdateSupplierRequest request,
        CancellationToken ct = default)
    {
        var command = new UpdateSupplierCommand(
            id,
            request.CompanyName,
            request.ContactPerson,
            request.Phone,
            request.Email,
            request.Address,
            request.BankAccountDetails,
            request.IsActive);

        var result = await Sender.Send(command, ct);

        if (result.IsSuccess)
            return Ok(ApiResponse<SupplierDto>.Ok(result.Value, "Supplier updated successfully."));

        if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(ApiResponse<SupplierDto>.Fail(result.Error));

        return BadRequest(ApiResponse<SupplierDto>.Fail(result.Error));
    }

    /// <summary>Soft-delete a supplier. Blocked if the supplier has open or sent purchase orders.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSupplier([FromRoute] Guid id, CancellationToken ct = default)
    {
        var command = new DeleteSupplierCommand(id);
        var result = await Sender.Send(command, ct);

        if (result.IsSuccess)
            return Ok(ApiResponse.Ok("Supplier deleted successfully."));

        if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(ApiResponse.Fail(result.Error));

        return BadRequest(ApiResponse.Fail(result.Error));
    }

    /// <summary>Get all purchase orders raised against a specific supplier.</summary>
    [HttpGet("{supplierId:guid}/purchase-orders")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetSupplierPurchaseOrders(
        [FromRoute] Guid supplierId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => NotImplementedResponse();

    /// <summary>Create a new purchase order for restocking from a supplier.</summary>
    [HttpPost("purchase-orders")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult CreatePurchaseOrder([FromBody] object request)
        => NotImplementedResponse();
}

/// <summary>Request body for updating a supplier. The route ID is taken from the URL.</summary>
public sealed record UpdateSupplierRequest(
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email,
    string? Address,
    string? BankAccountDetails,
    bool IsActive);
