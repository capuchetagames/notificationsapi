# Notifications API

A microservice responsible for handling notification events in a distributed system. This API consumes events from RabbitMQ and sends notifications (currently simulated by logging to console) for user registration and payment confirmations.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=flat&logo=rabbitmq&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=microsoft-sql-server&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Running the Application](#-running-the-application)
- [API Documentation](#-api-documentation)
- [Project Structure](#-project-structure)
- [Technologies](#-technologies)
- [Kubernetes Deployment](#-kubernetes-deployment)
- [Development](#-development)
- [License](#-license)

## ✨ Features

- **Event-Driven Architecture**: Consumes events from RabbitMQ message broker
- **Welcome Notifications**: Sends welcome emails when new users are created
- **Payment Notifications**: Sends purchase confirmation notifications when payments are approved
- **Database Persistence**: Stores notification history in SQL Server
- **Health Checks**: Built-in health check endpoints for monitoring
- **Swagger Documentation**: Interactive API documentation with Swagger and ReDoc
- **Docker Support**: Fully containerized with Docker and Docker Compose
- **Kubernetes Ready**: Complete Kubernetes deployment configurations included
- **Entity Framework Core**: Database migrations and ORM support

## 🏗️ Architecture

The application follows a clean architecture pattern with three main layers:

```
NotificationsApi/
├── NotificationsApi    # API layer (Controllers, Services, Configuration)
├── Core               # Domain layer (Entities, DTOs, Interfaces)
└── Infrastructure     # Data layer (Repositories, Migrations, DbContext)
```

### Event Consumers

The API listens to two main event streams:

1. **User Events** (`users.events` exchange)
   - Queue: `notifications.users`
   - Routing Key: `user.*`
   - Action: Sends welcome notification

2. **Payment Events** (`payments.events` exchange)
   - Queue: `notifications.payments`
   - Routing Key: `payment.approved`
   - Action: Sends payment confirmation notification

## 📦 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or use the Docker Compose setup)
- [RabbitMQ](https://www.rabbitmq.com/download.html) (or use external RabbitMQ instance)

## 🚀 Installation

### Clone the Repository

```bash
git clone https://github.com/capuchetagames/notificationsapi.git
cd notificationsapi
```

### Restore Dependencies

```bash
dotnet restore
```

## ⚙️ Configuration

### Application Settings

Update `appsettings.json` with your configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1435;Database=Db.Notifications;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  },
  "RabbitMq": {
    "Host": "localhost",
    "User": "admin",
    "Password": "admin"
  }
}
```

### Environment Variables

For Docker deployments, configure these environment variables:

- `ASPNETCORE_ENVIRONMENT`: Set to `Development` or `Production`
- `ConnectionStrings__DefaultConnection`: SQL Server connection string
- `RabbitMq__Host`: RabbitMQ host address
- `RabbitMq__User`: RabbitMQ username
- `RabbitMq__Password`: RabbitMQ password

## 🏃 Running the Application

### Using Docker Compose (Recommended)

The easiest way to run the entire stack:

```bash
docker-compose up -d
```

This will start:
- SQL Server on port `1435`
- Notifications API on port `5100`

The API will be available at: `http://localhost:5100`

### Running Locally

1. **Start SQL Server and RabbitMQ** (or update connection strings to point to your instances)

2. **Apply Database Migrations**:
   ```bash
   cd NotificationsApi
   dotnet ef database update
   ```

3. **Run the Application**:
   ```bash
   dotnet run --project NotificationsApi
   ```

The API will be available at: `http://localhost:5000` (or as configured)

## 📚 API Documentation

Once the application is running in Development mode, you can access:

- **Swagger UI**: `http://localhost:5100/swagger`
- **ReDoc**: `http://localhost:5100/api-docs`
- **Health Check**: `http://localhost:5100/health`

### Available Endpoints

- `GET /health` - Health check endpoint
- `GET /api/notifications` - Retrieve notifications (check controllers for available endpoints)

## 📁 Project Structure

```
notificationsapi/
├── NotificationsApi/           # Main API project
│   ├── Configs/               # RabbitMQ configuration
│   ├── Controllers/           # API controllers
│   ├── Service/               # Background services and consumers
│   ├── Program.cs             # Application entry point
│   └── appsettings.json       # Configuration files
├── Core/                      # Core domain layer
│   ├── Dtos/                 # Data transfer objects
│   ├── Entity/               # Domain entities
│   ├── Models/               # Domain models
│   └── Repository/           # Repository interfaces
├── Infrastructure/            # Infrastructure layer
│   ├── Migrations/           # EF Core migrations
│   └── Repository/           # Repository implementations
├── NotificationsApi.Tests/   # Unit and integration tests
├── k8s/                      # Kubernetes configurations
│   ├── notifications-deployment.yaml
│   ├── notifications-service.yaml
│   ├── sql-deployment.yaml
│   └── ...
├── docker-compose.yaml       # Docker Compose configuration
├── Dockerfile               # Docker image definition
└── README.md               # This file
```

## 🛠️ Technologies

- **Framework**: .NET 8.0
- **Language**: C# 12
- **Database**: SQL Server 2022
- **ORM**: Entity Framework Core 8.0
- **Message Broker**: RabbitMQ 7.2
- **API Documentation**: Swagger/OpenAPI
- **Validation**: FluentValidation
- **Containerization**: Docker
- **Orchestration**: Kubernetes

## ☸️ Kubernetes Deployment

The project includes complete Kubernetes configurations in the `k8s/` directory.

### Deploy to Kubernetes

1. **Configure Environment**:
   ```bash
   cd k8s
   chmod +x *.sh
   ./env.sh
   ```

2. **Deploy Database**:
   ```bash
   ./k8s-deploy-db.sh
   ```

3. **Deploy API**:
   ```bash
   ./k8s-deploy-api.sh
   ```

Or deploy everything at once:
```bash
./k8s-start-all-deploy.sh
```

### Available Kubernetes Scripts

- `k8s-deploy-api.sh` - Deploy the Notifications API
- `k8s-deploy-db.sh` - Deploy SQL Server database
- `k8s-dev-api.sh` - Deploy API in development mode
- `k8s-start-all-deploy.sh` - Deploy all services (production)
- `k8s-start-all-dev.sh` - Deploy all services (development)
- `k8s-delete-all.sh` - Remove all deployments

## 💻 Development

### Build the Project

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Apply Migrations

```bash
cd NotificationsApi
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Create New Migration

```bash
dotnet ef migrations add YourMigrationName --project Infrastructure --startup-project NotificationsApi
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Made with ❤️ by Capucheta Games**
