using EcommerceHub.Modules.Promotions.Domain.Interfaces;
using EcommerceHub.Modules.Promotions.Infrastructure.Persistence;
using EcommerceHub.Modules.Promotions.Infrastructure.Persistence.Repositories;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Promotions;

public sealed class PromotionsModule : IModule
{
    public string Name => "Promotions";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PromotionsDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "promotions")));

        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<IFlashSaleRepository, FlashSaleRepository>();
        services.AddScoped<IBannerRepository, BannerRepository>();
        services.AddScoped<IPromotionsUnitOfWork, PromotionsUnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PromotionsModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(PromotionsModule).Assembly);

        return services;
    }
}
