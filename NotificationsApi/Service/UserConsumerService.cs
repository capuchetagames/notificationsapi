using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;

namespace NotificationsApi.Service;


public class UserEventsConsumer : BackgroundService
{
    private readonly INotificationsRepository _notificationsRepository;
    private readonly IRabbitMqConsumer _consumer;

    public UserEventsConsumer(IRabbitMqConsumer consumer, INotificationsRepository notificationsRepository)
    {
        _consumer = consumer;
        _notificationsRepository = notificationsRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.ConsumeAsync<UserCreatedEvent>(
            exchange: "users.events",
            queue: "notifications.users",
            routingKey: "user.*",
            handler: Handle,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    private Task Handle(UserCreatedEvent userCreatedEvent)
    {
        var notificationMessage = new Notifications()
        {
            UserId = userCreatedEvent.UserId,
            Message = "Mensagem de Boas Vindas!",
            Subject = "Boas Vindas!",
            Type = "Email",
            Status = "Sent"
        };
        
        _notificationsRepository.Add(notificationMessage);

        Console.WriteLine($"📧 Welcome email para {userCreatedEvent.Name} | {userCreatedEvent.Email}");
        
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consumer.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}