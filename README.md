# Overview
Esta é uma API de vendas desenvolvida para o desafio técnico do **DeveloperStore**.  
O objetivo é implementar um **CRUD completo de vendas** com aplicação de regras de negócio (descontos progressivos, limite de itens, cancelamentos) e suporte a eventos de domínio.  

A solução foi construída utilizando boas práticas de **DDD**, **CQRS** e **Clean Architecture simplificada**, garantindo separação de responsabilidades e manutenibilidade.

---

## Tech Stack
- **.NET 8 (Minimal API)** – backend principal  
- **C# 12**  
- **Swagger / OpenAPI** – documentação e testes  
- **InMemory Repository** – persistência simples para o desafio  

---

## Frameworks
- **MediatR 12** → implementação de CQRS e pipeline behaviors  
- **FluentValidation** → validações dinâmicas de inputs  
- **Swashbuckle.AspNetCore** → geração de Swagger/OpenAPI  
- **Microsoft.Extensions.Logging** → logs e publicação de eventos simulados  

---

## Project Structure

DeveloperStore.Sales/
├── DeveloperStore.Sales.sln
├── README.md
└── src/
├── Sales.Domain/
│ ├── Sale.cs
│ ├── SaleItem.cs
│ ├── DiscountPolicy.cs
│ └── DomainException.cs
│
├── Sales.Application/
│ ├── Dtos.cs
│ ├── Interfaces.cs
│ ├── Mediator.cs # Commands & Queries (records)
│ ├── Mapping.cs # Extensions → ToResponse()
│ ├── Validators.cs # FluentValidation
│ ├── ValidationBehavior.cs # MediatR pipeline
│ └── Handlers/
│ ├── Sales/
│ │ ├── CreateSaleHandler.cs
│ │ ├── UpdateSaleHandler.cs
│ │ ├── CancelSaleHandler.cs
│ │ ├── GetSaleByIdHandler.cs
│ │ └── ListSalesHandler.cs
│ └── Items/
│ ├── AddItemHandler.cs
│ ├── UpdateItemHandler.cs
│ └── CancelItemHandler.cs
│
├── Sales.Infrastructure/
│ ├── InMemorySaleRepository.cs # Persistência em memória
│ └── LoggerEventPublisher.cs # Publicação de eventos via log
│
└── Sales.Api/
├── Program.cs # Minimal API + endpoints
└── Sales.Api.csproj

```

**Camadas**
- **Domain**: entidades e regras de negócio (sem dependências externas).
- **Application**: DTOs, CQRS (MediatR), validações e mapeamentos.
- **Infrastructure**: implementações (repositório InMemory, publisher).
- **Api**: endpoints HTTP, configuração de DI/Swagger e exception handling.

## How to Run

### Pré-requisitos
- **.NET 8 SDK** instalado

### Passos
1. Restaurar pacotes:
```bash
   dotnet restore
```
2. Compilar a solução:
```bash
   dotnet build
```
3. Executar a API:   
 ```bash
  dotnet run --project src/Sales.Api/Sales.Api.csproj
 ```