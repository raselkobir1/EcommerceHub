using EcommerceHub.Modules.Reviews.Domain.Interfaces;
using EcommerceHub.Modules.Reviews.Infrastructure.Persistence;
using EcommerceHub.Modules.Reviews.Infrastructure.Persistence.Repositories;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Reviews;

public sealed class ReviewsModule : IModule
{
    public string Name => "Reviews";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ReviewsDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "reviews")));

        services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
        services.AddScoped<IReviewsUnitOfWork, ReviewsUnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ReviewsModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(ReviewsModule).Assembly);

        return services;
    }
}
