using Bla.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bla.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => { c.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); c.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); });
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
