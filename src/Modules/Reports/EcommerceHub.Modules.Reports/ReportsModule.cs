using EcommerceHub.Shared.Kernel.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Reports;

public sealed class ReportsModule : IModule
{
    public string Name => "Reports";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ReportsModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(ReportsModule).Assembly);
        return services;
    }
}
