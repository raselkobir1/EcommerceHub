using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Modules.Suppliers.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Suppliers.Application.Queries.GetSuppliers;

internal sealed class GetSuppliersQueryHandler(SuppliersDbContext db)
    : IRequestHandler<GetSuppliersQuery, Result<PagedResult<SupplierListDto>>>
{
    public async Task<Result<PagedResult<SupplierListDto>>> Handle(GetSuppliersQuery request, CancellationToken ct)
    {
        var query = db.Suppliers
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.CompanyName.ToLower().Contains(term) ||
                s.ContactPerson.ToLower().Contains(term) ||
                s.Email.ToLower().Contains(term) ||
                s.Phone.Contains(term));
        }

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        var total = await query.CountAsync(ct);
        var skip = (request.Page - 1) * request.PageSize;

        var suppliers = await query
            .OrderBy(s => s.CompanyName)
            .Skip(skip)
            .Take(request.PageSize)
            .Include(s => s.PurchaseOrders)
                .ThenInclude(po => po.Payments)
            .ToListAsync(ct);

        var dtos = suppliers.Select(s => new SupplierListDto(
            s.Id,
            s.CompanyName,
            s.ContactPerson,
            s.Phone,
            s.Email,
            s.IsActive,
            s.PurchaseOrders.Count,
            s.OutstandingBalance,
            s.CreatedAt)).ToList();

        return Result.Success(PagedResult<SupplierListDto>.Create(dtos, total, request.Page, request.PageSize));
    }
}
