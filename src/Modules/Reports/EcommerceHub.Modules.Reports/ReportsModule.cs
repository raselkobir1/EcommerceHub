using EcommerceHub.Modules.Reports.Infrastructure.Persistence;
using EcommerceHub.Shared.Kernel.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Reports;

public sealed class ReportsModule : IModule
{
    public string Name => "Reports";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        // Read-only reporting context — no migrations, no schema ownership.
        services.AddDbContext<ReportsDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ReportsModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(ReportsModule).Assembly);
        return services;
    }
}
