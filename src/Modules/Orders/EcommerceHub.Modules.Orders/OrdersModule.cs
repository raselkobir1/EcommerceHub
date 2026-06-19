using EcommerceHub.Modules.Orders.Domain.Interfaces;
using EcommerceHub.Modules.Orders.Infrastructure.Persistence;
using EcommerceHub.Modules.Orders.Infrastructure.Persistence.Repositories;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Orders;

public sealed class OrdersModule : IModule
{
    public string Name => "Orders";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "orders")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrdersUnitOfWork, OrdersUnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrdersModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(OrdersModule).Assembly);

        return services;
    }
}
