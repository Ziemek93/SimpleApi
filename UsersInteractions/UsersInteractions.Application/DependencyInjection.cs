using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UsersInteractions.Application.Middleware;

namespace UsersInteractions.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices
        (this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        var thisAssembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(thisAssembly);
            x.AddOpenBehavior(typeof(ValidationHandlers<,>));
            // x.AddOpenBehavior(typeof(LoggingBehavior<,>)); 
        });

        
        return services;
    }

}
