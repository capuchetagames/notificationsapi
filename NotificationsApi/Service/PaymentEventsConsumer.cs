using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;

namespace NotificationsApi.Service;

public class PaymentEventsConsumer : BackgroundService
{
    private readonly IRabbitMqConsumer _consumer;
    
    private readonly INotificationsRepository _notificationsRepository;

    public PaymentEventsConsumer(IRabbitMqConsumer consumer, INotificationsRepository notificationsRepository)
    {
        _consumer = consumer;
        _notificationsRepository = notificationsRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.ConsumeAsync<PaymentProcessedEvent>(
            exchange: "payments.events",
            queue: "notifications.payments",
            routingKey: "payment.*",
            handler: Handle,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task Handle(PaymentProcessedEvent paymentProcessedEvent)
    {
        var notificationMessage = new Notifications()
        {
            UserId = paymentProcessedEvent.UserId,
            Message = $"Mensagem de Status da Compra: {paymentProcessedEvent.Status}",
            Subject = "Compra Efetuada",
            Type = "Email",
            Status = "Sent"
        };
        
        _notificationsRepository.Add(notificationMessage);

        Console.WriteLine($"💳 Mensagem de Status da Compra : {paymentProcessedEvent.Status} | {paymentProcessedEvent.Name} | {paymentProcessedEvent.Email}");
        
        return Task.CompletedTask;
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consumer.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
