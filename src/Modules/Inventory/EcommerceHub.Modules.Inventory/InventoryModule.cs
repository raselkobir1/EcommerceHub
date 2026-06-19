using EcommerceHub.Modules.Inventory.Domain.Interfaces;
using EcommerceHub.Modules.Inventory.Infrastructure.Persistence;
using EcommerceHub.Modules.Inventory.Infrastructure.Persistence.Repositories;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Inventory;

public sealed class InventoryModule : IModule
{
    public string Name => "Inventory";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "inventory")));

        services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();
        services.AddScoped<IInventoryUnitOfWork, InventoryUnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InventoryModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(InventoryModule).Assembly);

        return services;
    }
}
