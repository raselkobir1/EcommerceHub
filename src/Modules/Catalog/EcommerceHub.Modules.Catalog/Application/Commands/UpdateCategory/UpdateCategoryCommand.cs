using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Slug,
    Guid? ParentId,
    string? Description,
    string? ImageUrl,
    bool IsActive,
    int SortOrder = 0) : IRequest<Result<CategoryDto>>;
