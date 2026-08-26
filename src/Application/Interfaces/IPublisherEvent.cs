namespace Application.Interfaces;

public interface IPublisherEvent
{
    Task ProduceEventAsync(string topic,
        string key,
        string value,
        CancellationToken cancellationToken = default);
}
