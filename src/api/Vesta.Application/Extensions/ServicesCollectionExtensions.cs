using Microsoft.Extensions.DependencyInjection;
using Vesta.Application.Interfaces;
using Vesta.Application.Services;

namespace Vesta.Application.Extensions;

public static class ServicesCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddVestaApplication()
        {
            return services.AddScoped<IUserService, UserService>();
        }
    }
}