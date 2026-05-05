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
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? _settings.Host,
            UserName = _settings.User,
            Password = _settings.Password,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };
        
        
        int[] retryDelays = [5, 10, 20, 30];
        foreach (var delay in retryDelays)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync(cancellationToken);
                break;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"[RabbitMQ] Falha ao conectar, tentando novamente em {delay}s... ({ex.Message})");
                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
            }
        }

        if (_connection is null || !_connection.IsOpen)
        {
            throw new InvalidOperationException("[RabbitMQ] Não foi possível estabelecer conexão após todas as tentativas.");
        }
            
        
        Console.WriteLine($"[RabbitMQ] Conectado com sucesso! ({_connection.Endpoint.HostName}:{_connection.Endpoint.Port})");
        
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
