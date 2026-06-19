using EcommerceHub.Modules.Catalog.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Catalog.Application.Queries.GetProducts;

public sealed record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    string? SearchTerm = null,
    string? Status = null,
    bool? IsFeatured = null) : IRequest<Result<PagedResult<ProductListDto>>>;
