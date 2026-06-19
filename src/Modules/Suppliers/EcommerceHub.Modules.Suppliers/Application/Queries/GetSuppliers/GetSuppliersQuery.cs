using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Application.Queries.GetSuppliers;

public sealed record GetSuppliersQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    bool? IsActive = null) : IRequest<Result<PagedResult<SupplierListDto>>>;
