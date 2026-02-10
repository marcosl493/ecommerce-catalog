using Serilog;

namespace WebApi;

public static partial class Program
{
    public static IServiceCollection AddSerilog(this IServiceCollection services)
    {
        services.AddSerilog((services, lc) => lc
                .ReadFrom.Configuration(services.GetRequiredService<IConfiguration>())
                .ReadFrom.Services(services));
        return services;
    }
}
