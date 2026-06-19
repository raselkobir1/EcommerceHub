using EcommerceHub.Modules.Customers.Domain.Entities;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Customers.Domain.Interfaces;

public interface ICustomerProfileRepository : IRepository<CustomerProfile>
{
    Task<CustomerProfile?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
}
