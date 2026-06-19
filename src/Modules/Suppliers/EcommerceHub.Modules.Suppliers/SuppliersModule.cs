using EcommerceHub.Modules.Suppliers.Domain.Interfaces;
using EcommerceHub.Modules.Suppliers.Infrastructure.Persistence;
using EcommerceHub.Modules.Suppliers.Infrastructure.Persistence.Repositories;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Suppliers;

public sealed class SuppliersModule : IModule
{
    public string Name => "Suppliers";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SuppliersDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "suppliers")));

        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IUnitOfWork, SuppliersUnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SuppliersModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(SuppliersModule).Assembly);

        return services;
    }
}
