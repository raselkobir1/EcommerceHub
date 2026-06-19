using EcommerceHub.Modules.Customers.Domain.Entities;
using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Shared.Kernel.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EcommerceHub.Modules.Customers.Infrastructure.Persistence.Repositories;

internal sealed class CustomerProfileRepository(CustomersDbContext context)
    : BaseRepository<CustomerProfile, CustomersDbContext>(context), ICustomerProfileRepository
{
    public async Task<CustomerProfile?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        => await context.CustomerProfiles.FirstOrDefaultAsync(p => p.Id == customerId, ct);
}
