using Microsoft.AspNetCore.Authentication.JwtBearer;

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
        /// <returns>
        /// The <see cref="IServiceCollection"/> instance with Vesta API services configured. This enables further
        /// chaining of service configuration methods.
        /// </returns>
        public IServiceCollection AddVestaApi()
        {
            services.AddControllers();
            services.AddOpenApi();
            services
                .AddEndpointsApiExplorer()
                .AddAuthentication();
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
        private void AddAuthentication()
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer
                (options =>
                    {
                        options.Authority = "https://dev-0o6diwm4bjc1cwiw.us.auth0.com/";
                        options.Audience = "https://localhost:7141";
                    }
                );
        }
    }
}