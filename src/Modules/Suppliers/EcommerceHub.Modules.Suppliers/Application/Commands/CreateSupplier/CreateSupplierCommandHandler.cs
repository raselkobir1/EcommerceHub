using EcommerceHub.Modules.Suppliers.Application.DTOs;
using EcommerceHub.Modules.Suppliers.Domain.Entities;
using EcommerceHub.Modules.Suppliers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Application.Commands.CreateSupplier;

internal sealed class CreateSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken ct)
    {
        var emailNormalised = request.Email.ToLowerInvariant();

        var emailExists = await supplierRepository.ExistsAsync(
            s => s.Email == emailNormalised && !s.IsDeleted, ct);

        if (emailExists)
            return Result.Failure<SupplierDto>($"A supplier with email '{emailNormalised}' already exists.");

        var supplier = Supplier.Create(
            request.CompanyName,
            request.ContactPerson,
            request.Phone,
            request.Email,
            request.Address,
            request.BankAccountDetails);

        await supplierRepository.AddAsync(supplier, ct);
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
            TotalPurchaseOrders: 0,
            TotalOwed: 0m,
            TotalPaid: 0m,
            OutstandingBalance: 0m,
            supplier.CreatedAt,
            supplier.UpdatedAt));
    }
}
