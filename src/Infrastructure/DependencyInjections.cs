using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<CatalogDbContext>((sp, options) =>
            options.UseNpgsql
            (
                sp.GetRequiredService<IConfiguration>().BuildConnectionString(nameof(CatalogDbContext)),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName))
            );
        services
            .AddLogging()
            .AddRepositories();
        return services;
    }
    private static string BuildConnectionString(this IConfiguration configuration, string name)
    {
        var username = configuration.GetValue<string>("DB_USERNAME") ?? throw new InvalidOperationException("Database username not found.");
        var password = configuration.GetValue<string>("DB_PASSWORD") ?? throw new InvalidOperationException("Database password not found.");
        var connectionString = configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Connection string '{name}' not found.");
        var sb = new StringBuilder(connectionString);
        sb.Append($"Username={username};Password={password}");
        return sb.ToString();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }


}
