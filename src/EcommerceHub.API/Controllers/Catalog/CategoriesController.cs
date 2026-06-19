using EcommerceHub.Modules.Catalog.Application.Commands.CreateCategory;
using EcommerceHub.Modules.Catalog.Application.Commands.DeleteCategory;
using EcommerceHub.Modules.Catalog.Application.Commands.UpdateCategory;
using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Application.Queries.GetCategories;
using EcommerceHub.Modules.Catalog.Application.Queries.GetCategoryById;
using EcommerceHub.Shared.Kernel.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceHub.API.Controllers.Catalog;

[Route("api/categories")]
public sealed class CategoriesController(MediatR.ISender sender) : ApiController(sender)
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var result = await Sender.Send(new GetCategoriesQuery(), ct);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}", Name = "GetCategoryById")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new GetCategoryByIdQuery(id), ct);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken ct)
    {
        var result = await Sender.Send(command, ct);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Value.Id },
                ApiResponse<CategoryDto>.Ok(result.Value));
        return BadRequest(ApiResponse<object>.Fail(result.Error!));
    }

    /// <summary>Update an existing category's fields. Requires Manager role.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse<object>.Fail("Route id does not match command id."));

        var result = await Sender.Send(command, ct);

        if (result.IsSuccess)
            return Ok(ApiResponse<CategoryDto>.Ok(result.Value, "Category updated successfully."));

        if (result.Error!.Contains("not found"))
            return NotFound(ApiResponse<object>.Fail(result.Error));

        return BadRequest(ApiResponse<object>.Fail(result.Error));
    }

    /// <summary>Soft-delete a category. Fails if it has children or assigned products. Requires Manager role.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Manager")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await Sender.Send(new DeleteCategoryCommand(id), ct);
        return HandleResult(result);
    }
}
