# NotificationsAPI

API de notificações responsável pelo envio de e-mails de boas-vindas e confirmação de compra.

## 📋 Sobre o Projeto

Este serviço faz parte de um sistema maior e é responsável por gerenciar o envio de notificações por e-mail. Atualmente, o sistema simula o envio de e-mails registrando as mensagens no console, facilitando o desenvolvimento e testes.

## 🚀 Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core** - Para construção da API RESTful
- **C#** - Linguagem de programação
- **Docker** - Containerização da aplicação
- **Kubernetes** - Orquestração de containers
- **xUnit** - Framework de testes unitários

## 📁 Estrutura do Projeto

```
notificationsapi/
├── Core/                          # Lógica de negócio e modelos de domínio
├── Infrastructure/                # Configuração de infraestrutura
├── NotificationsApi/              # Projeto principal da API
│   ├── Controllers/               # Controladores da API
│   ├── Service/                   # Camada de serviços
│   └── Configs/                   # Arquivos de configuração
├── NotificationsApi.Tests/        # Testes unitários
└── k8s/                           # Arquivos de deployment do Kubernetes
```

## 🔧 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (opcional, para executar em container)
- [Docker Compose](https://docs.docker.com/compose/install/) (opcional)

## 🏃 Como Executar

### Executando Localmente

1. Clone o repositório:
```bash
git clone https://github.com/capuchetagames/notificationsapi.git
cd notificationsapi
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Execute a aplicação:
```bash
dotnet run --project NotificationsApi
```

A API estará disponível em `https://localhost:5001` ou `http://localhost:5000`.

### Executando com Docker

1. Construa a imagem Docker:
```bash
docker build -t notificationsapi .
```

2. Execute o container:
```bash
docker run -p 5000:8080 notificationsapi
```

### Executando com Docker Compose

```bash
docker-compose up
```

## 📬 Endpoints da API

A API expõe endpoints para envio de notificações por e-mail:

- **E-mail de Boas-vindas** - Envia uma mensagem de boas-vindas para novos usuários
- **E-mail de Confirmação de Compra** - Envia confirmação de compra para clientes

> **Nota:** Atualmente, os e-mails são simulados através de logs no console. Nenhum e-mail real é enviado.

## ⚙️ Configuração

As configurações da aplicação podem ser encontradas em:

- `appsettings.json` - Configurações gerais
- `appsettings.Development.json` - Configurações específicas para desenvolvimento

## 🐳 Deploy com Kubernetes

Os arquivos de configuração do Kubernetes estão disponíveis no diretório `k8s/`. Para fazer o deploy:

```bash
kubectl apply -f k8s/
```
