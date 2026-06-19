using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Modules.Auth.Domain.Entities;

namespace EcommerceHub.Modules.Auth.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<Customer?> GetByRefreshTokenAsync(string token, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default);
}
