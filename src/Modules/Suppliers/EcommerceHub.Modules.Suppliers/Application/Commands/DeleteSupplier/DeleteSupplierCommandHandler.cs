using EcommerceHub.Modules.Suppliers.Domain.Enums;
using EcommerceHub.Modules.Suppliers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Common;
using MediatR;

namespace EcommerceHub.Modules.Suppliers.Application.Commands.DeleteSupplier;

internal sealed class DeleteSupplierCommandHandler(
    ISupplierRepository supplierRepository,
    IPurchaseOrderRepository purchaseOrderRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeleteSupplierCommand, Result>
{
    public async Task<Result> Handle(DeleteSupplierCommand request, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(request.SupplierId, ct);

        if (supplier is null || supplier.IsDeleted)
            return Result.Failure("Supplier not found.");

        var hasOpenOrders = await purchaseOrderRepository.ExistsAsync(
            po => po.SupplierId == request.SupplierId &&
                  (po.Status == PurchaseOrderStatus.Draft || po.Status == PurchaseOrderStatus.Sent),
            ct);

        if (hasOpenOrders)
            return Result.Failure("Cannot delete a supplier that has open purchase orders. Close or cancel all orders first.");

        var deletedBy = currentUser.Email ?? "system";
        supplier.SoftDelete(deletedBy);

        supplierRepository.Update(supplier);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
