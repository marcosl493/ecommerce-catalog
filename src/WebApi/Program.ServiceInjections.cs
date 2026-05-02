using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;
using Serilog.Events;

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
        .WriteTo.Elasticsearch([new Uri(elasticUrl)], opts =>
{
    opts.MinimumLevel = LogEventLevel.Information;
	opts.DataStream = new DataStreamName("logs", indexName, "default");
	opts.BootstrapMethod = BootstrapMethod.None;
	opts.ConfigureChannel = channelOpts =>
	{
        channelOpts.BufferOptions = new BufferOptions
        {
            InboundBufferMaxSize = 100_000,
            OutboundBufferMaxSize = 1_000,
            OutboundBufferMaxLifetime = TimeSpan.FromSeconds(5),
            ExportMaxConcurrency = null
	};};
}, transport =>
{
	 transport.Authentication(new BasicAuthentication(elasticUsername, elasticPassword));
})
        .CreateLogger();
        
        return services;
    }
}
