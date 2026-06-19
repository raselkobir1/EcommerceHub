using EcommerceHub.Modules.Catalog.Application.Commands.CreateCategory;
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

    [HttpGet("{id:guid}")]
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
}
