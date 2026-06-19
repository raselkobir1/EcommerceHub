using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Queries.GetCategoryById;

internal sealed class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, ct);
        if (category is null)
            return Result.Failure<CategoryDto>($"Category '{request.Id}' not found.");

        return Result.Success(MapToDto(category));
    }

    private static CategoryDto MapToDto(Category category)
        => new(
            category.Id,
            category.Name,
            category.Slug,
            Description: null,
            category.ImageUrl,
            category.ParentId,
            category.IsActive,
            category.SortOrder,
            ProductCount: 0,
            category.Children.Select(MapToDto));
}
