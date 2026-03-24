# NotificationsAPI

API de notificações responsável pelo envio de e-mails de boas-vindas e confirmação de compra, orientada a eventos via RabbitMQ.

## 📋 Sobre o Projeto

Este serviço faz parte de um sistema de microsserviços e é responsável por gerenciar o envio de notificações por e-mail. O serviço consome eventos de filas RabbitMQ, persiste as notificações em um banco SQL Server e aciona a **EmailSenderLambda** (AWS Lambda) para o envio real dos e-mails.

### Fluxo de funcionamento

```
[Outro microsserviço] --> RabbitMQ --> [NotificationsAPI] --> EmailSenderLambda (AWS Lambda)
                                              |
                                         SQL Server
                                    (persiste notificações)
```

- **Evento `user.*`** (exchange `users.events`, fila `notifications.users`): dispara e-mail de boas-vindas.
- **Evento `payment.approved`** (exchange `payments.events`, fila `notifications.payments`): dispara e-mail de status da compra.

## 🚀 Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core** - Para construção da API RESTful
- **C#** - Linguagem de programação
- **RabbitMQ** (`RabbitMQ.Client 7.x`) - Mensageria e consumo de eventos
- **Entity Framework Core 8** - ORM com SQL Server e Lazy Loading
- **SQL Server 2022** - Banco de dados relacional
- **Serilog** + **New Relic Log Enricher** - Logging estruturado com integração ao New Relic
- **FluentValidation** - Validação de modelos
- **Swagger / ReDoc** - Documentação interativa da API
- **Docker** - Containerização da aplicação
- **Docker Compose** - Orquestração local com SQL Server
- **Kubernetes** - Orquestração de containers em produção
- **xUnit** - Framework de testes unitários

## 📁 Estrutura do Projeto

```
notificationsapi/
├── Core/                          # Lógica de negócio e modelos de domínio
│   ├── Dtos/                      # Eventos recebidos (UserCreatedEvent, PaymentProcessedEvent)
│   ├── Entity/                    # Entidades do domínio (Notifications)
│   ├── Models/                    # Interfaces de serviços (IRabbitMqConsumer, etc.)
│   └── Repository/                # Interfaces de repositório
├── Infrastructure/                # Implementação de infraestrutura
│   ├── Migrations/                # Migrations do Entity Framework Core
│   └── Repository/                # ApplicationDbContext e repositórios EF
├── NotificationsApi/              # Projeto principal da API
│   ├── Configs/                   # RabbitMqConsumer e RabbitMqSettings
│   ├── Controllers/               # Controladores da API
│   └── Service/                   # Background Services (UserEventsConsumer, PaymentEventsConsumer)
├── NotificationsApi.Tests/        # Testes unitários
├── docker-compose.yaml            # Compose com API + SQL Server
└── k8s/                           # Arquivos de deployment do Kubernetes
```

## 🔧 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/install/) — para SQL Server e RabbitMQ locais
- Instância do **RabbitMQ** acessível (pode ser local ou via rede compartilhada de microsserviços)

## ⚙️ Configuração

As configurações da aplicação encontram-se em:

- `appsettings.json` - Configurações gerais
- `appsettings.Development.json` - Configurações específicas para desenvolvimento

### Variáveis de ambiente relevantes

| Variável | Descrição |
|---|---|
| `ConnectionStrings__DefaultConnection` | String de conexão com o SQL Server |
| `RABBITMQ_HOST` | Host do RabbitMQ (sobrescreve `RabbitMq:Host`) |
| `EmailSenderLambda__BaseUrl` | URL base da AWS Lambda responsável pelo envio de e-mails |
| `Jwt__Key` | Chave JWT para autenticação |

Exemplo de configuração no `appsettings.json`:

```json
{
  "RabbitMq": {
    "Host": "localhost",
    "User": "guest",
    "Password": "guest"
  },
  "EmailSenderLambda": {
    "BaseUrl": "https://<lambda-url>"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1435;Database=Db.Notifications;User Id=sa;Password=rooot1234!!;TrustServerCertificate=True;"
  }
}
```

## 🏃 Como Executar

### Executando Localmente

1. Clone o repositório:
```bash
git clone https://github.com/capuchetagames/notificationsapi.git
cd notificationsapi
```

2. Suba o SQL Server localmente (via Docker Compose):
```bash
docker-compose up notifications-db -d
```

3. Restaure as dependências:
```bash
dotnet restore
```

4. Execute a aplicação (as migrations são aplicadas automaticamente no ambiente Development):
```bash
dotnet run --project NotificationsApi
```

A API estará disponível em `http://localhost:5100`.

### Executando com Docker Compose

Sobe a API junto com o SQL Server em rede compartilhada (`app-network`):

```bash
docker-compose up
```

A API estará disponível em `http://localhost:5100`.

> **Nota:** O RabbitMQ deve estar disponível na rede `app-network` com o hostname `rabbitmq` (ou configure a variável `RABBITMQ_HOST`).

### Executando com Docker (imagem isolada)

1. Construa a imagem Docker:
```bash
docker build -t notificationsapi .
```

2. Execute o container:
```bash
docker run -p 5100:8080 notificationsapi
```

## 📬 Endpoints da API

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/health` | Health check da aplicação |
| `GET` | `/swagger` | Documentação Swagger (apenas em Development) |
| `GET` | `/api-docs` | Documentação ReDoc (apenas em Development) |

### Consumidores de eventos (Background Services)

| Serviço | Exchange | Fila | Routing Key | Ação |
|---|---|---|---|---|
| `UserEventsConsumer` | `users.events` | `notifications.users` | `user.*` | Envia e-mail de boas-vindas via EmailSenderLambda (`POST /api/emails/welcome`) |
| `PaymentEventsConsumer` | `payments.events` | `notifications.payments` | `payment.approved` | Envia e-mail de status da compra via EmailSenderLambda (`POST /api/emails/payment-status`) |

## 📊 Observabilidade

A aplicação utiliza **Serilog** com o enriquecedor do **New Relic** para logs estruturados no formato JSON, gravados em `logs/app.log.json` (com rotação diária). Os logs são compatíveis com o ingestion do New Relic Logs.

## 🐳 Deploy com Kubernetes

Os arquivos de configuração do Kubernetes estão disponíveis no diretório `k8s/`. Para fazer o deploy:

```bash
kubectl apply -f k8s/
```

Scripts auxiliares disponíveis em `k8s/`:

- `k8s-start-all-deploy.sh` — Sobe todos os recursos em modo deploy
- `k8s-start-all-dev.sh` — Sobe todos os recursos em modo dev
- `k8s-delete-all.sh` — Remove todos os recursos do cluster
