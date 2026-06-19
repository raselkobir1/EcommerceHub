using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Modules.Catalog.Domain.Entities;
using EcommerceHub.Modules.Catalog.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.CreateCategory;

internal sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (await categoryRepository.SlugExistsAsync(request.Slug, null, ct))
            return Result.Failure<CategoryDto>($"A category with slug '{request.Slug}' already exists.");

        if (request.ParentId.HasValue &&
            !await categoryRepository.ExistsAsync(c => c.Id == request.ParentId.Value, ct))
            return Result.Failure<CategoryDto>($"Parent category '{request.ParentId}' not found.");

        var category = Category.Create(
            request.Name,
            request.Slug,
            request.ParentId,
            request.ImageUrl,
            request.SortOrder);

        await categoryRepository.AddAsync(category, ct);
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
            Enumerable.Empty<CategoryDto>());
}
