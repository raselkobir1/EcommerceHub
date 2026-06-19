using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string Slug,
    Guid? ParentId,
    string? Description,
    string? ImageUrl,
    int SortOrder = 0) : IRequest<Result<CategoryDto>>;
