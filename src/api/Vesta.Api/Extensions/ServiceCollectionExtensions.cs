using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Vesta.Infrastructure.Persistence;

namespace Vesta.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring services related to the Vesta API in an ASP.NET Core application.    
/// </summary>
/// <remarks>
/// This class contains extension methods for registering controllers, OpenAPI documentation, endpoint API explorer,
/// and authentication services with an IServiceCollection. These methods are intended to simplify the setup of common
/// Vesta API dependencies during application startup.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core Vesta API services, including controllers, OpenAPI documentation, endpoint exploration, and JWT Bearer
    /// authentication, to the specified service collection.
    /// </summary>
    /// <remarks>
    /// This method configures the service collection with essential services required for a typical ASP.NET Core Web
    /// API, including authentication using JWT Bearer tokens. Call this method during application startup to ensure
    /// all necessary services are registered.
    /// </remarks>
    /// <param name="services">
    /// The service collection to which the Vesta API services are added. Cannot be null.
    /// </param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds and configures Vesta API services, including controllers, OpenAPI documentation, endpoint API
        /// explorer, and authentication, to the service collection.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>
        /// The <see cref="IServiceCollection"/> instance with Vesta API services configured. This enables further
        /// chaining of service configuration methods.
        /// </returns>
        public IServiceCollection AddVestaApi(IConfigurationManager configuration)
        {
            services.AddControllers();
            services.AddOpenApi();
            services
                .AddEndpointsApiExplorer()
                .AddVestaAuthentication(configuration)
                .AddDatabase(configuration);

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
        private IServiceCollection AddVestaAuthentication(IConfigurationManager configuration)
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

        /// <summary>
        /// Adds and configures the database context for the application using PostgreSQL as the database provider.
        /// </summary>
        /// <param name="configuration">The configuration manager used to retrieve the connection string.</param>
        private void AddDatabase(IConfigurationManager configuration)
        {
            services.AddDbContext<VestaDbContext>
            (options => options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
            );
        }
    }
}