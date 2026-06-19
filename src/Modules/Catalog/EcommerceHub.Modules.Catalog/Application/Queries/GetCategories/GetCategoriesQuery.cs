using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Queries.GetCategories;

public sealed record GetCategoriesQuery() : IRequest<Result<IEnumerable<CategoryDto>>>;
