using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Modules.Suppliers.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Suppliers.Application.Queries.GetSupplierById;

internal sealed class GetSupplierByIdQueryHandler(SuppliersDbContext db)
    : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(GetSupplierByIdQuery request, CancellationToken ct)
    {
        var supplier = await db.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == request.SupplierId && !s.IsDeleted)
            .Include(s => s.PurchaseOrders)
                .ThenInclude(po => po.Payments)
            .FirstOrDefaultAsync(ct);

        if (supplier is null)
            return Result.Failure<SupplierDto>("Supplier not found.");

        return Result.Success(new SupplierDto(
            supplier.Id,
            supplier.CompanyName,
            supplier.ContactPerson,
            supplier.Phone,
            supplier.Email,
            supplier.Address,
            supplier.BankAccountDetails,
            supplier.IsActive,
            supplier.PurchaseOrders.Count,
            supplier.TotalOwed,
            supplier.TotalPaid,
            supplier.OutstandingBalance,
            supplier.CreatedAt,
            supplier.UpdatedAt));
    }
}
