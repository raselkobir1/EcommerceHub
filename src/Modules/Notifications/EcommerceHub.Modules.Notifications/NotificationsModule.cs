using EcommerceHub.Modules.Notifications.Application.Services;
using EcommerceHub.Modules.Notifications.Infrastructure.Services;
using EcommerceHub.Shared.Kernel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Modules.Notifications;

public sealed class NotificationsModule : IModule
{
    public string Name => "Notifications";

    public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(NotificationsModule).Assembly));
        return services;
    }
}
