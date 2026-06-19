using EcommerceHub.Modules.Catalog.Application.Commands.ArchiveProduct;
using EcommerceHub.Modules.Catalog.Application.Commands.CreateProduct;
using EcommerceHub.Modules.Catalog.Application.Commands.DeleteProduct;
using EcommerceHub.Modules.Catalog.Application.Commands.PublishProduct;
using EcommerceHub.Modules.Catalog.Application.Commands.UpdateProduct;
using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Application.Queries.GetProductById;
using EcommerceHub.Modules.Catalog.Application.Queries.GetProducts;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Catalog;

[Route("api/products")]
public sealed class ProductsController(MediatR.ISender sender) : ApiController(sender)
{
    /// <summary>
    /// Get a paginated, filterable list of products.
    /// Supports filtering by category, brand, free-text search, and status.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? brandId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? status = null,
        [FromQuery] bool? isFeatured = null,
        CancellationToken ct = default)
    {
        var query = new GetProductsQuery(page, pageSize, categoryId, brandId, searchTerm, status, isFeatured);
        var result = await Sender.Send(query, ct);

        if (result.IsSuccess)
            return Ok(ApiResponse<PagedResult<ProductListDto>>.Ok(result.Value));

        return BadRequest(ApiResponse<PagedResult<ProductListDto>>.Fail(result.Error!));
    }

    /// <summary>Get full details of a single product by its ID.</summary>
    [HttpGet("{id:guid}", Name = "GetProductById")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id, CancellationToken ct)
    {
        var query = new GetProductByIdQuery(id);
        var result = await Sender.Send(query, ct);

        if (result.IsSuccess)
            return Ok(ApiResponse<ProductDetailDto>.Ok(result.Value));

        return NotFound(ApiResponse<ProductDetailDto>.Fail(result.Error!));
    }

    /// <summary>Create a new product with variants. Requires Manager role.</summary>
    [HttpPost]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);

        if (result.IsSuccess)
            return CreatedAtRoute(
                "GetProductById",
                new { id = result.Value.Id },
                ApiResponse<ProductDetailDto>.Ok(result.Value, "Product created successfully."));

        return BadRequest(ApiResponse<ProductDetailDto>.Fail(result.Error!));
    }

    /// <summary>Update an existing product's core fields. Requires Manager role.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute] Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<object>.Fail("Route id does not match command id."));

        var result = await Sender.Send(command, ct);

        if (result.IsSuccess)
            return Ok(ApiResponse<ProductDetailDto>.Ok(result.Value, "Product updated successfully."));

        if (result.Error!.Contains("not found"))
            return NotFound(ApiResponse<object>.Fail(result.Error));

        return BadRequest(ApiResponse<object>.Fail(result.Error));
    }

    /// <summary>Soft-delete a product. Requires Manager role.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new DeleteProductCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Publish a product making it visible to customers. Requires Manager role.</summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PublishProduct([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new PublishProductCommand(id), ct);
        return HandleResult(result);
    }

    /// <summary>Archive a product removing it from the public catalog. Requires Manager role.</summary>
    [HttpPost("{id:guid}/archive")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveProduct([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new ArchiveProductCommand(id), ct);
        return HandleResult(result);
    }
}
