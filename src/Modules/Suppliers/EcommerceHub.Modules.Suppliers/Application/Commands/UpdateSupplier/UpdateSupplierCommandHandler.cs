using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Modules.Suppliers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Application.Commands.UpdateSupplier;

internal sealed class UpdateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSupplierCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(UpdateSupplierCommand request, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.Id, ct);

        if (supplier is null || supplier.IsDeleted)
            return Result.Failure<SupplierDto>("Supplier not found.");

        var emailNormalised = request.Email.ToLowerInvariant();

        var emailTaken = await supplierRepository.ExistsAsync(
            s => s.Email == emailNormalised && s.Id != request.Id && !s.IsDeleted, ct);

        if (emailTaken)
            return Result.Failure<SupplierDto>($"Another supplier with email '{emailNormalised}' already exists.");

        supplier.Update(
            request.CompanyName,
            request.ContactPerson,
            request.Phone,
            request.Email,
            request.Address,
            request.BankAccountDetails,
            request.IsActive);

        supplierRepository.Update(supplier);
        await unitOfWork.SaveChangesAsync(ct);

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
