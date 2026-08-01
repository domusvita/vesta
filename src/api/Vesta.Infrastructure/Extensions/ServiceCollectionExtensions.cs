using Microsoft.Extensions.DependencyInjection;

namespace Vesta.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddVestaInfrastructure()
        {
            return services;
        }
    }
}