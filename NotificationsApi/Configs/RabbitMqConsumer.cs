using System.Text.Json;
using Core.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationsApi.Configs;


public class RabbitMqConsumer : IRabbitMqConsumer
{
    private readonly RabbitMqSettings _settings;
    
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumer(RabbitMqSettings settings)
    {
        _settings = settings;
    }

    public async Task ConsumeAsync<T>(string exchange, string queue, string routingKey, Func<T, Task> handler, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            UserName = _settings.User,
            Password = _settings.Password,
            AutomaticRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken:cancellationToken);

        await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Topic, true, cancellationToken:cancellationToken);
        await _channel.QueueDeclareAsync(queue, true, false, false, null, cancellationToken:cancellationToken);
        await _channel.QueueBindAsync(queue, exchange, routingKey, cancellationToken:cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<T>(ea.Body.Span);
                await handler(message!);

                await _channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
            }
            catch
            {
                // futuramente DLQ
                await _channel.BasicNackAsync(ea.DeliveryTag, false, false, cancellationToken);
            }
        };

        await _channel.BasicConsumeAsync(queue, false, consumer, cancellationToken);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.CloseAsync();

        if (_connection is not null)
            await _connection.CloseAsync();
    }
}
