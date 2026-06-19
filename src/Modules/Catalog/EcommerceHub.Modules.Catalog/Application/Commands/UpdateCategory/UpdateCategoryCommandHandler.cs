using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, ct);
        if (category is null)
            return Result.Failure<CategoryDto>($"Category '{request.Id}' not found.");

        if (await categoryRepository.SlugExistsAsync(request.Slug, request.Id, ct))
            return Result.Failure<CategoryDto>($"A category with slug '{request.Slug}' already exists.");

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == request.Id)
                return Result.Failure<CategoryDto>("A category cannot be its own parent.");

            if (!await categoryRepository.ExistsAsync(c => c.Id == request.ParentId.Value, ct))
                return Result.Failure<CategoryDto>($"Parent category '{request.ParentId}' not found.");
        }

        category.Update(
            request.Name,
            request.Slug,
            request.ParentId,
            request.ImageUrl,
            request.SortOrder,
            request.IsActive);

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(ct);

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
