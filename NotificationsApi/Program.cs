using Core.Models;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.Extensions.Options;
using NewRelic.LogEnrichers.Serilog;
using NotificationsApi.Configs;
using NotificationsApi.Service;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithNewRelicLogsInContext() // método do pacote
    .WriteTo.File(
        path: "logs/app.log.json",
        formatter: new NewRelicFormatter(),
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddScoped<INotificationsRepository, NotificationsRepository>();

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<IRabbitMqConsumer>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
    return new RabbitMqConsumer(settings);
});

builder.Services.AddHostedService<UserEventsConsumer>();
builder.Services.AddHostedService<PaymentEventsConsumer>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseReDoc(c =>
    {
        c.DocumentTitle = "REDOC API Documentation";
        c.SpecUrl = "/swagger/v1/swagger.json";
    });
    
    
}

//app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("Notification API is up");

app.Run();
