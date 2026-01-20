using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;

namespace NotificationsApi.Service;


public class UserEventsConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqConsumer _consumer;

    public UserEventsConsumer(IRabbitMqConsumer consumer)
    {
        _consumer = consumer;
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
            Status = "Sent",
            DeliveredAt = DateTime.Now
        };
        
        Console.WriteLine($"📧 Welcome email para >> {userCreatedEvent.UserId} | {userCreatedEvent.Name} | {userCreatedEvent.Email}");

        using var scope = _scopeFactory.CreateScope();

        var repo = scope.ServiceProvider
            .GetRequiredService<INotificationsRepository>();
        
            repo.Add(notificationMessage);

        
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consumer.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}