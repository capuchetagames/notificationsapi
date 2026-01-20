namespace Core.Models;

public interface IRabbitMqConsumer : IAsyncDisposable
{
    Task ConsumeAsync<T>(
        string exchange,
        string queue,
        string routingKey,
        Func<T, Task> handler,
        CancellationToken cancellationToken
    );
}
