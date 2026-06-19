using EcommerceHub.Modules.Suppliers.Domain.Entities;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Suppliers.Domain.Interfaces;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<Supplier?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken ct = default);
}

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetByPoNumberAsync(string poNumber, CancellationToken ct = default);
    Task<PurchaseOrder?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken ct = default);
}
