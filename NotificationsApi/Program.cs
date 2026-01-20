using Core.Models;
using Core.Repository;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotificationsApi.Configs;
using NotificationsApi.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json").Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
}, ServiceLifetime.Scoped);

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

app.Run();
