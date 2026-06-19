using EcommerceHub.Modules.Payments.Domain.Interfaces;
using EcommerceHub.Modules.Payments.Infrastructure.Persistence;
using EcommerceHub.Modules.Payments.Infrastructure.Persistence.Repositories;
using EcommerceHub.Shared.Kernel.Abstractions;
using EcommerceHub.Shared.Kernel.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Payments;

public sealed class PaymentsModule : IModule
{
    public string Name => "Payments";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentsDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "payments")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork, PaymentsUnitOfWork>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PaymentsModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(PaymentsModule).Assembly);

        return services;
    }
}
