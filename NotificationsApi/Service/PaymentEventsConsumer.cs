using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;

namespace NotificationsApi.Service;

public class PaymentEventsConsumer : BackgroundService
{
    private readonly IRabbitMqConsumer _consumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public PaymentEventsConsumer(IRabbitMqConsumer consumer, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _consumer = consumer;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.ConsumeAsync<PaymentProcessedEvent>(
            exchange: "payments.events",
            queue: "notifications.payments",
            routingKey: "payment.approved",
            handler: Handle,
            cancellationToken: stoppingToken
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task Handle(PaymentProcessedEvent paymentProcessedEvent)
    {
        var notificationMessage = new Notifications()
        {
            UserId = paymentProcessedEvent.UserId,
            Message = $"Mensagem de Status da Compra: {paymentProcessedEvent.Status}",
            Subject = "Compra Efetuada",
            Type = "Email",
            Status = "Sent"
        };

        using var scope = _scopeFactory.CreateScope();

        var repo = scope.ServiceProvider
            .GetRequiredService<INotificationsRepository>();

            repo.Add(notificationMessage);
        
        // Acionando a Lambda da AWS (EmailSenderLambda) para pagamentos
        try
        {
            using var httpClient = new HttpClient();
            var emailPayload = new 
            { 
                Status = paymentProcessedEvent.Status, 
                Name = paymentProcessedEvent.Name, 
                Email = paymentProcessedEvent.Email 
            };
            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(emailPayload), System.Text.Encoding.UTF8, "application/json");

            var baseUrl = _configuration["EmailSenderLambda:BaseUrl"];
            await httpClient.PostAsync($"{baseUrl}/api/emails/payment-status", jsonContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao acionar a API Lambda de E-mail de Pagamento: {ex.Message}");
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _consumer.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
