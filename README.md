# 🚀 Tellus - Sistema de Automação

## 📋 Sobre o Projeto
Tellus é uma aplicação web completa desenvolvida com **ASP.NET Core 8.0** (backend) e **Blazor WebAssembly** (frontend).  
O sistema implementa uma **arquitetura em camadas**, com separação clara de responsabilidades, **autenticação JWT** e **operações CRUD completas**.

---

## 🏗️ Arquitetura e Tecnologias

| Camada             | Tecnologia         | Descrição                                   |
|--------------------|--------------------|---------------------------------------------|
| **Frontend**       | Blazor WebAssembly | Interface web interativa desenvolvida em C# |
| **Backend API**    | ASP.NET Core 8.0   | API RESTful baseada em controllers          |
| **Banco de Dados** | PostgreSQL         | Banco relacional principal                  |
| **ORM**            | Dapper             | Micro-ORM para mapeamento e acesso a dados  |
| **Autenticação**   | JWT Tokens         | Autenticação stateless e segura             |
| **Testes**         | xUnit              | Testes unitários e de integração            |


## 📦 Pré-requisitos

### 1. .NET SDK 8.0
**Download:** [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

**Verificação da instalação:**
dotnet --version

Deve retornar: 8.0.x
🛠️ Instalação e Configuração
## 1. Clonar o Repositório
**git clone git@github.com:EduardoGape/Tellus.git**


## 2. Configurar o Banco de Dados
CREATE DATABASE tellus;

## 3. Configurar Connection String

Editar o arquivo TellusAPI/Presentation/appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tellus;Username=postgres;Password=postgres"
  },
  "JWT": {
    "SecretKey": "0ae1c7c01a714cd9b45134ac180ead05b3036572b97a4878a6ea571006c2e929"
  }
}

## 4. Restaurar Dependências
# Backend
cd TellusAPI
dotnet restore

# Frontend
cd ../TellusWeb
dotnet restore

### 🏃 Executando a Aplicação
## Backend (API)
# cd TellusAPI/Presentation
# dotnet run

URL: https://localhost:5296


## Frontend (Blazor)
# cd TellusWeb/TellusWeb.App
# dotnet run

URL:https://localhost:6060


## 🧪 Executando Testes
Testes Unitários
cd TellusAPI/Tests/TellusAPI.UnitTests
cd TellusAPI/Tests/TellusAPI.IntegrationTests

dotnet test

### 📁 Estrutura do Projeto
Tellus/
├── TellusAPI/                     # Backend API
│   ├── Application/               # DTOs, Services, Interfaces
│   ├── Domain/                    # Entities, Common
│   ├── Infrastructure/            # Configurações, Database
│   ├── Presentation/              # Controllers, Program.cs
│   └── Tests/                     # Testes unitários e de integração
└── TellusWeb/                     # Frontend Blazor
    ├── TellusWeb.App/             # Aplicação Blazor
    ├── TellusWeb.Application/     # Serviços do frontend
    ├── TellusWeb.Domain/          # Entidades e DTOs
    └── TellusWeb.Infrastructure/  # Configurações HTTP

🔧 Desenvolvimento
Hot Reload
# Backend
cd TellusAPI/Presentation
dotnet watch run

# Frontend
cd TellusWeb/TellusWeb.App
dotnet watch run

📚 Documentação da API

Acesse quando a aplicação estiver rodando:
Swagger UI: https://localhost:5296/swagger

