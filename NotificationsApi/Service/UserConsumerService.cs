using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;

namespace NotificationsApi.Service;


public class UserEventsConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMqConsumer _consumer;
    private readonly IConfiguration _configuration;

    public UserEventsConsumer(IRabbitMqConsumer consumer, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _consumer = consumer;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
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
    private async Task Handle(UserCreatedEvent userCreatedEvent)
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
        
            // Acionando a Lambda da AWS (EmailSenderLambda)
            try
            {
                using var httpClient = new HttpClient();
                var emailPayload = new { UserId = userCreatedEvent.UserId, Name = userCreatedEvent.Name, Email = userCreatedEvent.Email };
                var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(emailPayload), System.Text.Encoding.UTF8, "application/json");

                var baseUrl = _configuration["EmailSenderLambda:BaseUrl"];
                await httpClient.PostAsync($"{baseUrl}/api/emails/welcome", jsonContent);
            }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao acionar a API Lambda de E-mail: {ex.Message}");
        }

        using var scope = _scopeFactory.CreateScope();

        var repo = scope.ServiceProvider
            .GetRequiredService<INotificationsRepository>();
        
            repo.Add(notificationMessage);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consumer.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}