using EcommerceHub.Modules.Customers.Domain.Entities;
using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Customers.Infrastructure.Persistence.Repositories;

internal sealed class CustomerAddressRepository(CustomersDbContext context)
    : BaseRepository<CustomerAddress, CustomersDbContext>(context), ICustomerAddressRepository
{
    public async Task<IEnumerable<CustomerAddress>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await context.CustomerAddresses.Where(a => a.CustomerId == customerId).ToListAsync(ct);

    public async Task<CustomerAddress?> GetDefaultAsync(Guid customerId, CancellationToken ct = default)
        => await context.CustomerAddresses.FirstOrDefaultAsync(a => a.CustomerId == customerId && a.IsDefault, ct);
}
