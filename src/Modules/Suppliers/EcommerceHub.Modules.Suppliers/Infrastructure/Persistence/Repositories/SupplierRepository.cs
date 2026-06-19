using EcommerceHub.Modules.Suppliers.Domain.Entities;
using EcommerceHub.Modules.Suppliers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Suppliers.Infrastructure.Persistence.Repositories;

internal sealed class SupplierRepository(SuppliersDbContext context)
    : BaseRepository<Supplier, SuppliersDbContext>(context), ISupplierRepository
{
    public async Task<Supplier?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await context.Suppliers.FirstOrDefaultAsync(s => s.Email == email, ct);

    public async Task<IEnumerable<Supplier>> GetActiveAsync(CancellationToken ct = default)
        => await context.Suppliers.Where(s => s.IsActive).OrderBy(s => s.CompanyName).ToListAsync(ct);
}
