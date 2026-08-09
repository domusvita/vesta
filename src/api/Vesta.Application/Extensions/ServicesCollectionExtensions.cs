using Microsoft.Extensions.DependencyInjection;
using Vesta.Application.Interfaces;
using Vesta.Application.Services;

namespace Vesta.Application.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddVestaApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IQuestionService, QuestionService>();
        return services;
    }
}