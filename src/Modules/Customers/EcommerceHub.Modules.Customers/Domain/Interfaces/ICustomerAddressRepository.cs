using EcommerceHub.Modules.Customers.Domain.Entities;
using EcommerceHub.Shared.Kernel.Abstractions;

namespace EcommerceHub.Modules.Customers.Domain.Interfaces;

public interface ICustomerAddressRepository : IRepository<CustomerAddress>
{
    Task<IEnumerable<CustomerAddress>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<CustomerAddress?> GetDefaultAsync(Guid customerId, CancellationToken ct = default);
}
