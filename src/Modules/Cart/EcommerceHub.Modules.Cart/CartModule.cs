using EcommerceHub.Modules.Cart.Domain.Interfaces;
using EcommerceHub.Modules.Cart.Infrastructure.Persistence;
using EcommerceHub.Modules.Cart.Infrastructure.Persistence.Repositories;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace EcommerceHub.Modules.Cart;

public sealed class CartModule : IModule
{
    public string Name => "Cart";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CartDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "cart")));

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnection));

        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartUnitOfWork, CartUnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CartModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(CartModule).Assembly);

        return services;
    }
}
