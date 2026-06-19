using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Queries.GetCategories;

internal sealed class GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<CategoryDto>>>
{
    public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var roots = await categoryRepository.GetRootCategoriesAsync(ct);

        var dtos = roots.Select(MapToDto);

        return Result.Success(dtos);
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
