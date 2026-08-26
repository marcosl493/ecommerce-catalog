using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging;

public sealed class KafkaProducer
    (
        ProducerConfig config,
        ILogger<KafkaProducer> logger
    ) : IPublisherEvent, IDisposable
{
    private readonly IProducer<string, string> _producer = new ProducerBuilder<string, string>(config).Build();
    private readonly ILogger<KafkaProducer> _logger = logger;

    public Task ProduceEventAsync(string topic, string key, string value, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Producing message to topic '{Topic}' with key '{Key}'", topic, key);

        _producer.Produce(topic, new Message<string, string> { Key = key, Value = value }, deliveryReport =>
        {
            if (deliveryReport.Status == PersistenceStatus.Persisted || deliveryReport.Error == null || !deliveryReport.Error.IsError)
            {
                _logger.LogDebug("Message delivered to {TopicPartitionOffset} (topic: '{Topic}', key: '{Key}')",
                    deliveryReport.TopicPartitionOffset, topic, key);
            }
            else
            {
                _logger.LogError("Failed to deliver message to topic '{Topic}' with key '{Key}'", topic, key);
            }
        });

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        try
        {
            _logger.LogInformation("Flushing pending messages and disposing Kafka producer");
            _producer.Flush(TimeSpan.FromSeconds(10));
            _logger.LogInformation("Flush complete");
        }
        finally
        {
            _producer.Dispose();
        }
    }
    public sealed class Options
    {
        public const string SectionName = "Kafka:Producer";
        public string BootstrapServers { get; set; } = null!;
        public Acks Acks { get; set; } = Acks.All;
        public int MessageTimeoutMs { get; set; } = 30000;
    }
}
