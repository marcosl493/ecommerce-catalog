using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text;

namespace Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {


        services
            .AddOptions<MongoDbContext.Options>()
            .Bind(configuration.GetSection(MongoDbContext.Options.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var objectSerializer = new ObjectSerializer(BsonSerializer.LookupDiscriminatorConvention(typeof(object)), GuidRepresentation.Standard);
        BsonSerializer.RegisterSerializer(objectSerializer);

        // Register MongoDB class maps for domain entities (id mapping etc.)
        Infrastructure.Persistence.Mappings.MongoMappings.Register();

        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        services.AddSingleton(sp => sp.GetRequiredService<IMongoDbContext>().GetMongoDatabase());

        services
            .AddLogging()
            .AddRepositories();

        services.AddSingleton<IStorageService, AwsS3StorageService>();

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
        services.AddScoped<IProductRepository, ProductMongoRepository>();
        return services;
    }


}
