using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjections
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjections).Assembly));
        return services;
    }
}
