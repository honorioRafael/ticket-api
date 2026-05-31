# Ticket API - Sistema de Venda de Ingressos

Este projeto consiste em um sistema de venda de ingressos construído com **.NET 10** utilizando as práticas de **DDD (Domain-Driven Design)**, **Arquitetura Hexagonal (Ports & Adapters)**, **EF Core** e **PostgreSQL**.

A solução é composta por dois contextos delimitados (Bounded Contexts) independentes e uma biblioteca compartilhada (TicketApi.Common):
1. **Events (Microserviço 1)**: Responsável pela administração de locais (venues), ciclo de vida dos eventos e tipos de ingresso.
2. **Sales (Microserviço 2)**: Responsável pela criação de pedidos, confirmação de pagamentos, validação de ingressos e controle de expiração.
3. **TicketApi.Common (Compartilhado)**: Contém classes base comuns, como a exceção de domínio base `DomainException` e o middleware global de tratamento de erros `ErrorHandlingMiddleware`.

---

## Estrutura do Projeto

```
src/
├── TicketApi.Common/         (Class Lib - Componentes Compartilhados)
│
├── Contexts.Events/
│   ├── Events.API/           (Web API - Entrypoint da API de Eventos)
│   ├── Events.Application/   (Class Lib - Casos de Uso e Validações)
│   ├── Events.Domain/        (Class Lib - Modelos de Domínio Puros)
│   └── Events.Infrastructure/ (Class Lib - EF Core, Repositórios, Migrations)
│
├── Contexts.Sales/
│   ├── Sales.API/           (Web API - Entrypoint da API de Vendas e Jobs)
│   ├── Sales.Application/   (Class Lib - Casos de Uso e Validações)
│   ├── Sales.Domain/        (Class Lib - Modelos de Domínio Puros e Réplicas)
│   └── Sales.Infrastructure/ (Class Lib - EF Core, Repositórios, Migrations)
```
## Como Executar a Aplicação

### Opção A: Pelo Visual Studio (Recomendado)
Para executar e debugar as duas APIs (`Events.API` e `Sales.API`) simultaneamente:
1. Abra o arquivo de solução `TicketApi.sln` no **Visual Studio 2022** (ou superior).
2. Na janela **Gerenciador de Soluções** (Solution Explorer), clique com o botão direito sobre a solução `TicketApi` e selecione **Propriedades** (Properties).
3. Sob **Propriedades Comuns** (Common Properties), clique em **Projeto de Inicialização** (Startup Project).
4. Selecione a opção **Vários projetos de inicialização** (Multiple startup projects).
5. Defina a ação para **Iniciar** (Start) nos seguintes projetos:
   - `Events.API`
   - `Sales.API`
6. Clique em **Aplicar** e **OK**.
7. Pressione **F5** ou o botão **Iniciar** para rodar ambas as APIs juntas diretamente no Windows.

### Opção B: Pela Linha de Comando (CLI)
Abra duas janelas de terminal na raiz do projeto e execute uma API em cada uma:

* **Para a API de Eventos:**
  ```bash
  dotnet run --project src/Contexts.Events/Events.API/Events.API.csproj
  ```

* **Para a API de Vendas:**
  ```bash
  dotnet run --project src/Contexts.Sales/Sales.API/Sales.API.csproj
  ```

---

## Portas de Acesso e Swagger

Ao iniciar localmente, as APIs estarão disponíveis nas seguintes portas locais:
* **Events API**: [http://localhost:5001](http://localhost:5001)
  * Swagger UI: [http://localhost:5001/swagger](http://localhost:5001/swagger)
* **Sales API**: [http://localhost:5002](http://localhost:5002)
  * Swagger UI: [http://localhost:5002/swagger](http://localhost:5002/swagger)

---

## Comandos Úteis do Entity Framework (EF Core)

A ferramenta CLI `dotnet-ef` é necessária para gerenciar migrations e atualizar o banco de dados.

### 1. Criar Novas Migrations
Sempre que fizer alterações nas entidades do domínio, gere uma nova migration apontando para o DbContext específico:

* **Para o Contexto de Eventos:**
  ```bash
  dotnet ef migrations add NOME_DA_MIGRATION -p src/Contexts.Events/Events.Infrastructure/Events.Infrastructure.csproj -s src/Contexts.Events/Events.API/Events.API.csproj --context EventsDbContext
  ```

* **Para o Contexto de Vendas:**
  ```bash
  dotnet ef migrations add NOME_DA_MIGRATION -p src/Contexts.Sales/Sales.Infrastructure/Sales.Infrastructure.csproj -s src/Contexts.Sales/Sales.API/Sales.API.csproj --context SalesDbContext
  ```

### 2. Aplicar Migrations no Banco de Dados
Com as strings de conexão já configuradas nos projetos, você pode executar os comandos para aplicar as migrations diretamente:

* **Atualizar Esquema de Eventos (`events`):**
  ```bash
  dotnet ef database update --project src/Contexts.Events/Events.Infrastructure/Events.Infrastructure.csproj --startup-project src/Contexts.Events/Events.API/Events.API.csproj --context EventsDbContext
  ```

* **Atualizar Esquema de Vendas (`sales`):**
  ```bash
  dotnet ef database update --project src/Contexts.Sales/Sales.Infrastructure/Sales.Infrastructure.csproj --startup-project src/Contexts.Sales/Sales.API/Sales.API.csproj --context SalesDbContext
  ```

---

## Compilar a Solução Localmente
Para garantir que todas as camadas e projetos estão compilando sem erros:
```bash
dotnet build TicketApi.sln
```
