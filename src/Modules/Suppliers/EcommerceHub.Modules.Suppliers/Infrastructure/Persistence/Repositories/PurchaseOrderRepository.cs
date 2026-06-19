using EcommerceHub.Modules.Suppliers.Domain.Entities;
using EcommerceHub.Modules.Suppliers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Suppliers.Infrastructure.Persistence.Repositories;

internal sealed class PurchaseOrderRepository(SuppliersDbContext context)
    : BaseRepository<PurchaseOrder, SuppliersDbContext>(context), IPurchaseOrderRepository
{
    public async Task<PurchaseOrder?> GetByPoNumberAsync(string poNumber, CancellationToken ct = default)
        => await context.PurchaseOrders.Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.PoNumber == poNumber, ct);

    public async Task<PurchaseOrder?> GetWithDetailsAsync(Guid id, CancellationToken ct = default)
        => await context.PurchaseOrders.Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken ct = default)
        => await context.PurchaseOrders.Include(p => p.Items)
            .Where(p => p.SupplierId == supplierId)
            .OrderByDescending(p => p.CreatedAt).ToListAsync(ct);
}
