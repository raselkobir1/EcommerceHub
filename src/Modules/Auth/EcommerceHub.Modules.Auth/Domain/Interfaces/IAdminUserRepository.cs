using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Modules.Auth.Domain.Entities;

namespace EcommerceHub.Modules.Auth.Domain.Interfaces;

public interface IAdminUserRepository : IRepository<AdminUser>
{
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<AdminUser?> GetByRefreshTokenAsync(string token, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}
