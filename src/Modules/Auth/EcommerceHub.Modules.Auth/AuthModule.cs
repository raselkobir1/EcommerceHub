using EcommerceHub.Modules.Auth.Application.Services;
using EcommerceHub.Modules.Auth.Domain.Interfaces;
using EcommerceHub.Modules.Auth.Infrastructure.Persistence;
using EcommerceHub.Modules.Auth.Infrastructure.Persistence.Repositories;
using EcommerceHub.Modules.Auth.Infrastructure.Services;
using EcommerceHub.Shared.Kernel.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Auth;

public sealed class AuthModule : IModule
{
    public string Name => "Auth";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(opt =>
            opt.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsHistoryTable("__ef_migrations_auth", "auth")));

        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAuthUnitOfWork, AuthUnitOfWork>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddValidatorsFromAssembly(typeof(AuthModule).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AuthModule).Assembly));

        return services;
    }
}
