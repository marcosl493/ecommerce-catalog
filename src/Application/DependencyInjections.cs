using Application.Behaviours;
using Application.UseCases.CreateProduct;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjections
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjections).Assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingPipelineBehaviour<,>));

        services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
        return services;
    }
}
