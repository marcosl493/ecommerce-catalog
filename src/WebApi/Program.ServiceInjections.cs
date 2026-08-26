using Serilog;

namespace WebApi;

public static partial class Program
{
    public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticUrl = configuration["ELASTICSEARCH_URL"]!;
        var elasticUsername = configuration["ELASTICSEARCH_USERNAME"]!;
        var elasticPassword = configuration["ELASTICSEARCH_PASSWORD"]!;
        var indexName = configuration["ELASTICSEARCH_INDEX_NAME"]!;

        Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Debug()
         .Enrich.FromLogContext()
         .WriteTo.Console()
         .CreateLogger();

        return services;
    }
}
