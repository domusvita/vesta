using System.Text.Json;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vesta.Application.Extensions;
using Vesta.Functions.Extensions;
using Vesta.Functions.Middleware;
using Vesta.Infrastructure.Extensions;

var host = new HostBuilder()
    .ConfigureAppConfiguration
    ((context, config) =>
        {
            config.AddUserSecrets<Program>(optional: true);

            var builtConfig = config.Build();
            var keyVaultUrl = builtConfig["KeyVaultUrl"];

            if (!string.IsNullOrEmpty(keyVaultUrl))
            {
                config.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
            }
        }
    )
    .ConfigureFunctionsWebApplication
    (worker => { worker.UseMiddleware<JwtMiddleware>(); }
    )
    .ConfigureServices
    ((context, services) =>
        {
            services
                .AddVestaFunctions(context.Configuration)
                .AddVestaInfrastructure()
                .AddVestaApplication();

            services.Configure<WorkerOptions>(options =>
            {
                options.Serializer = new Azure.Core.Serialization.JsonObjectSerializer(
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );
            });
        }
    )
    .Build();

host.Run();