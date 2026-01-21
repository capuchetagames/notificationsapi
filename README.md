# Notifications API

A .NET 8 microservice responsible for handling notifications in an event-driven architecture. The service consumes events from RabbitMQ and sends notifications (currently simulated by logging to console) for user registration and payment confirmations.

## 📋 Overview

This API is part of a microservices ecosystem and handles:
- **Welcome emails** when users are created
- **Purchase confirmation emails** when payments are processed

Notifications are triggered by consuming messages from RabbitMQ exchanges and stored in a SQL Server database for tracking purposes.

## 🏗️ Architecture

The project follows Clean Architecture principles with the following structure:

- **NotificationsApi**: Main API project (Presentation layer)
- **Core**: Domain entities, DTOs, and repository interfaces
- **Infrastructure**: Data access implementation using Entity Framework Core
- **NotificationsApi.Tests**: Unit and integration tests

### Technology Stack

- **.NET 8.0**: Main framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM for data access
- **SQL Server**: Database
- **RabbitMQ**: Message broker for event-driven communication
- **Docker & Docker Compose**: Containerization
- **Kubernetes**: Orchestration (deployment scripts included)
- **Swagger/ReDoc**: API documentation

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) and Docker Compose
- [RabbitMQ](https://www.rabbitmq.com/) (or use Docker)
- SQL Server (or use Docker)

### Running with Docker Compose (Recommended)

The easiest way to run the application is using Docker Compose:

```bash
docker-compose up -d
```

This will start:
- **SQL Server** on port `1435`
- **Notifications API** on port `5100`

The API will be available at: `http://localhost:5100`

#### Health Check
```bash
curl http://localhost:5100/health
```

#### Swagger Documentation
Open your browser and navigate to:
- Swagger UI: `http://localhost:5100/swagger`
- ReDoc: `http://localhost:5100/api-docs`

### Running Locally (Development)

1. **Clone the repository**
   ```bash
   git clone https://github.com/capuchetagames/notificationsapi.git
   cd notificationsapi
   ```

2. **Set up the database**
   
   Start SQL Server using Docker:
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=rooot1234!!" \
     -p 1435:1433 --name sql_server \
     -d mcr.microsoft.com/mssql/server:2022-latest
   ```

3. **Configure RabbitMQ**
   
   Start RabbitMQ using Docker:
   ```bash
   docker run -d --name rabbitmq \
     -p 5672:5672 -p 15672:15672 \
     -e RABBITMQ_DEFAULT_USER=admin \
     -e RABBITMQ_DEFAULT_PASS=admin \
     rabbitmq:3-management
   ```

4. **Update connection strings**
   
   Edit `appsettings.Development.json` if needed to match your local setup.

5. **Run the application**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project NotificationsApi
   ```

The API will start on `http://localhost:5100` (or the port configured in `launchSettings.json`).

## 📦 Kubernetes Deployment

Kubernetes deployment scripts are available in the `/k8s` directory:

```bash
# Deploy everything
./k8s/k8s-start-all-deploy.sh

# Or deploy components individually
./k8s/k8s-deploy-db.sh      # Deploy SQL Server
./k8s/k8s-deploy-api.sh     # Deploy Notifications API

# For development
./k8s/k8s-start-all-dev.sh

# Clean up
./k8s/k8s-delete-all.sh
```

## 📡 Event Consumers

The API listens to the following RabbitMQ exchanges:

### User Events Consumer
- **Exchange**: `users.events`
- **Queue**: `notifications.users`
- **Routing Key**: `user.*`
- **Event**: `UserCreatedEvent`
- **Action**: Sends a welcome email notification

### Payment Events Consumer
- **Exchange**: `payments.events`
- **Queue**: `notifications.payments`
- **Routing Key**: `payment.approved`
- **Event**: `PaymentProcessedEvent`
- **Action**: Sends a purchase confirmation notification

## 🗃️ Database

The API uses SQL Server with Entity Framework Core for data persistence.

### Notifications Table Schema

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| UserId | int | User identifier |
| Message | string | Notification message content |
| Subject | string | Notification subject |
| Type | string | Notification type (e.g., "Email") |
| Status | string | Notification status (e.g., "Sent") |
| DeliveredAt | DateTime | Delivery timestamp |
| CreatedAt | DateTime | Creation timestamp |
| UpdatedAt | DateTime | Last update timestamp |

### Migrations

Migrations are automatically applied on application startup in Development environment.

To create a new migration:
```bash
dotnet ef migrations add MigrationName --project Infrastructure --startup-project NotificationsApi
```

To apply migrations manually:
```bash
dotnet ef database update --project Infrastructure --startup-project NotificationsApi
```

## 🔧 Configuration

Configuration is managed through `appsettings.json` and environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=notifications-db,1435;Database=Db.Notifications;User Id=sa;Password=rooot1234!!;TrustServerCertificate=True;"
  },
  "RabbitMq": {
    "Host": "rabbitmq",
    "User": "admin",
    "Password": "admin"
  }
}
```

### Environment Variables (Docker)

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | Development |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | - |
| `RabbitMq__Host` | RabbitMQ host | rabbitmq |
| `RabbitMq__User` | RabbitMQ username | admin |
| `RabbitMq__Password` | RabbitMQ password | admin |

## 🧪 Testing

Run the tests using:

```bash
dotnet test
```

Or run tests for a specific project:
```bash
dotnet test NotificationsApi.Tests
```

## 📚 API Documentation

Once the application is running, you can access:

- **Swagger UI**: Interactive API documentation at `/swagger`
- **ReDoc**: Alternative documentation viewer at `/api-docs`

## 🏗️ Project Structure

```
.
├── Core/                          # Domain layer
│   ├── Entity/                   # Domain entities
│   ├── Dtos/                     # Data transfer objects
│   ├── Models/                   # Domain models
│   └── Repository/               # Repository interfaces
├── Infrastructure/               # Infrastructure layer
│   ├── Repository/              # Repository implementations
│   └── Migrations/              # EF Core migrations
├── NotificationsApi/            # Presentation layer
│   ├── Controllers/             # API controllers
│   ├── Service/                 # Background services & consumers
│   ├── Configs/                 # Configuration classes
│   └── Program.cs              # Application entry point
├── NotificationsApi.Tests/      # Test project
├── k8s/                         # Kubernetes manifests
├── docker-compose.yaml          # Docker Compose configuration
└── Dockerfile                   # Docker image definition
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

Capucheta Games

## 🔗 Related Projects

This microservice is part of a larger ecosystem. Make sure the following services are running for full functionality:
- Users API (publishes `user.created` events)
- Payments API (publishes `payment.approved` events)
- RabbitMQ message broker

## 📞 Support

For support, please open an issue in the GitHub repository.
