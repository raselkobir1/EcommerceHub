using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceHub.Shared.Kernel.Abstractions;

public interface IModule
{
    string Name { get; }
    IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration);
}
