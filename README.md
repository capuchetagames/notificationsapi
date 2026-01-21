# 📧 Notifications API

API responsável por enviar (simulando, através de logs no console) e-mails de boas-vindas e confirmação de compra.

## 📋 Sobre o Projeto

Esta API faz parte de uma arquitetura de microsserviços e é responsável por processar notificações através de eventos consumidos via RabbitMQ. Quando um usuário é criado ou uma compra é confirmada, esta API recebe o evento e simula o envio de e-mails registrando as informações no console.

## 🚀 Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core** - Framework web
- **Entity Framework Core** - ORM para acesso ao banco de dados
- **SQL Server 2022** - Banco de dados relacional
- **RabbitMQ** - Sistema de mensageria para eventos
- **Docker & Docker Compose** - Containerização
- **Swagger/OpenAPI** - Documentação da API
- **ReDoc** - Documentação alternativa da API

## 🏗️ Arquitetura

O projeto está organizado em três camadas principais:

- **NotificationsApi** - Camada de apresentação (Controllers, Configurações)
- **Core** - Camada de domínio (Entidades, DTOs, Modelos, Repositórios)
- **Infrastructure** - Camada de infraestrutura (Implementação de repositórios)

## 📦 Pré-requisitos

- Docker e Docker Compose instalados
- OU
- .NET 8.0 SDK instalado
- SQL Server 2022
- RabbitMQ

## 🔧 Como Executar

### Usando Docker Compose (Recomendado)

1. Clone o repositório:
```bash
git clone https://github.com/capuchetagames/notificationsapi.git
cd notificationsapi
```

2. Execute com Docker Compose:
```bash
docker-compose up -d
```

3. A API estará disponível em: `http://localhost:5100`

### Executando Localmente

1. Configure a connection string no arquivo `appsettings.json`

2. Execute as migrações do banco de dados:
```bash
dotnet ef database update
```

3. Execute a aplicação:
```bash
dotnet run --project NotificationsApi
```

## 🔌 Endpoints

A API expõe endpoints para gerenciamento de notificações. Para ver a documentação completa:

- **Swagger UI**: `http://localhost:5100/swagger`
- **ReDoc**: `http://localhost:5100/api-docs`
- **Health Check**: `http://localhost:5100/health`

## 📊 Banco de Dados

O banco de dados SQL Server é executado em um container Docker e está configurado com:

- **Host**: localhost
- **Porta**: 1435
- **Usuário**: sa
- **Senha**: rooot1234!!
- **Database**: Db.Notifications

## 🐰 RabbitMQ

A API consome eventos de duas filas principais:

1. **Eventos de Usuário** - Processa eventos de criação de usuários e envia e-mails de boas-vindas
2. **Eventos de Pagamento** - Processa eventos de confirmação de pagamento e envia e-mails de confirmação

## 🧪 Testes

Para executar os testes:

```bash
dotnet test
```

## 📝 Licença

Este projeto está sob a licença especificada no arquivo [LICENSE](LICENSE).

## 👥 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.
