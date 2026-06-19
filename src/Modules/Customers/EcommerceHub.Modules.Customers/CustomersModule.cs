using EcommerceHub.Modules.Customers.Domain.Interfaces;
using EcommerceHub.Modules.Customers.Infrastructure.Persistence;
using EcommerceHub.Modules.Customers.Infrastructure.Persistence.Repositories;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Customers;

public sealed class CustomersModule : IModule
{
    public string Name => "Customers";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustomersDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "customers")));

        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>();
        services.AddScoped<ICustomersUnitOfWork, CustomersUnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CustomersModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(CustomersModule).Assembly);

        return services;
    }
}
