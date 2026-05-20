# NotificationsAPI

API de notificações responsável pelo envio de e-mails de boas-vindas e status de pagamento, orientada a eventos via RabbitMQ.

## Sobre o Projeto

Este serviço faz parte de um sistema de microsserviços e é responsável por:

- Consumir eventos no RabbitMQ
- Persistir notificações no PostgreSQL (via EF Core)
- Acionar a **EmailSenderLambda** para envio real dos e-mails
- Registrar logs estruturados em tabela DynamoDB

### Fluxo de funcionamento

```text
[Outro microsserviço] --> RabbitMQ --> [NotificationsAPI] --> EmailSenderLambda (AWS Lambda)
                                              |
                                         PostgreSQL
                                          (histórico)
                                              |
                                           DynamoDB
                                            (logs)
```

- **Evento `user.*`** (exchange `users.events`, fila `notifications.users`): dispara e-mail de boas-vindas.
- **Evento `payment.approved`** (exchange `payments.events`, fila `notifications.payments`): dispara e-mail de status de pagamento.

## Tecnologias Utilizadas

- **.NET 8.0 / ASP.NET Core**
- **C#**
- **RabbitMQ** (`RabbitMQ.Client 7.2.0`)
- **Entity Framework Core 8** + **Npgsql**
- **PostgreSQL 16**
- **AWS DynamoDB** (logging)
- **Swashbuckle** (Swagger + ReDoc)
- **Docker / Docker Compose**
- **Kubernetes**

## Estrutura do Projeto

```text
notificationsapi/
├── Core/                      # Entidades, DTOs, contratos e regras centrais
├── Infrastructure/            # DbContext, migrations e infraestrutura de dados
├── NotificationsApi/          # API, middlewares, consumidores e configs
├── NotificationsApi.Tests/    # Projeto de testes
├── docker-compose.api.yaml    # Compose da API
├── docker-compose.local.yaml  # Compose local (PostgreSQL + dependências de execução)
└── k8s/                       # Manifests e scripts Kubernetes
```

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) + Docker Compose
- RabbitMQ acessível na rede da aplicação
- DynamoDB (AWS ou local, conforme configuração)

## Configuração

Use os arquivos:

- `NotificationsApi/appsettings.json`
- `NotificationsApi/appsettings.Development.json`
- `.env.example`

### Variáveis de ambiente principais

| Variável | Descrição |
|---|---|
| `ConnectionStrings__DefaultConnection` / `DB_CONNECTION_STRING` | Conexão com PostgreSQL |
| `RABBITMQ_HOST` | Host do RabbitMQ |
| `EmailSenderLambda__BaseUrl` | URL base da Lambda de e-mail |
| `DynamoDb__LogTableName` | Nome da tabela de logs no DynamoDB |
| `DynamoDb__UseLocal` | Define uso de DynamoDB local |
| `DynamoDb__LocalUrl` | URL do DynamoDB local |
| `AWS_DEFAULT_REGION` | Região AWS usada pelo DynamoDB |
| `Jwt__Key` | Chave JWT |

## Como Executar

### 1) Executando localmente (app via `dotnet run`)

1. Suba o PostgreSQL local:

```bash
docker compose -f docker-compose.local.yaml up -d notifications-db
```

2. Restaure e execute a API:

```bash
dotnet restore NotificationsApi.sln
dotnet run --project NotificationsApi/NotificationsApi.csproj
```

API disponível em `http://localhost:5100`.

> Observação: RabbitMQ e DynamoDB precisam estar acessíveis conforme suas configurações locais.

### 2) Executando com Docker Compose (API)

```bash
docker compose -f docker-compose.api.yaml up -d --build
```

API disponível em `http://localhost:5100`.

> Para execução completa local, combine os arquivos conforme necessidade do ambiente (API + banco + serviços externos).

### 3) Executando com Docker (imagem isolada)

```bash
docker build -t notificationsapi .
docker run -p 5100:8080 notificationsapi
```

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/health` | Health check da aplicação |
| `GET` | `/swagger` | Swagger UI (Development) |
| `GET` | `/api-docs` | ReDoc (Development) |

## Consumidores de eventos (Background Services)

| Serviço | Exchange | Fila | Routing Key | Ação |
|---|---|---|---|---|
| `UserEventsConsumer` | `users.events` | `notifications.users` | `user.*` | Chama Lambda `POST /welcome` |
| `PaymentEventsConsumer` | `payments.events` | `notifications.payments` | `payment.approved` | Chama Lambda `POST /payment-status` |

## Observabilidade

A API utiliza logging via `ILogger` com provider customizado para DynamoDB e middleware de correlação de requisições.

## Deploy com Kubernetes

Os manifestos e scripts estão no diretório `k8s/`.

Exemplo:

```bash
kubectl apply -f k8s/
```
