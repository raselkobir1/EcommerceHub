using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Suppliers;

[Route("api/suppliers")]
[Authorize(Policy = "Manager")]
public sealed class SuppliersController(MediatR.ISender sender) : ApiController(sender)
{
    /// <summary>Get a paginated list of all suppliers.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetSuppliers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => NotImplementedResponse();

    /// <summary>Get details of a single supplier by ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetSupplierById([FromRoute] Guid id)
        => NotImplementedResponse();

    /// <summary>Create a new supplier record.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult CreateSupplier([FromBody] object request)
        => NotImplementedResponse();

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
