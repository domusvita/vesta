using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vesta.Application.Interfaces;
using Vesta.Infrastructure.Persistence;

namespace Vesta.Functions.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVestaFunctions(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddVestaDatabase(configuration)
            .AddVestaAuthentication(configuration);
    }

    private static IServiceCollection AddVestaDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VestaDbContext>
            (options => options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
            );

            services.AddScoped<IVestaDbContext>(sp => sp.GetRequiredService<VestaDbContext>());

            return services;
        }

        /// <summary>
        /// Configures JWT bearer authentication for the application using the specified authority and audience.
        /// </summary>
        /// <remarks>
        /// This method sets up authentication services to validate JWT tokens issued by the configured authority. It
        /// should be called during application startup to enable secure authentication for
        /// protected endpoints.
        /// </remarks>
        private static IServiceCollection AddVestaAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer
                (options =>
                    {
                        options.Authority = configuration["Auth0:Domain"];
                        options.Audience = configuration["Auth0:Audience"];
                    }
                );

            return services;
        }
}