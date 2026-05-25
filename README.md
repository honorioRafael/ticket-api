# Ticket API - Sistema de Venda de Ingressos

Este projeto consiste em um sistema de venda de ingressos construído com **.NET 10** utilizando as práticas de **DDD (Domain-Driven Design)**, **Arquitetura Hexagonal (Ports & Adapters)**, **EF Core** e **PostgreSQL**.

A solução é composta por dois contextos delimitados (Bounded Contexts) independentes e uma biblioteca de kernel compartilhado (SharedKernel):
1. **Events (Microserviço 1)**: Responsável pela administração de locais (venues), ciclo de vida dos eventos e tipos de ingresso.
2. **Sales (Microserviço 2)**: Responsável pela criação de pedidos, reservas temporárias de estoque (concorrência controlada por `xmin` no PostgreSQL), confirmação de pagamentos, validação de ingressos e controle de expiração.
3. **SharedKernel (Compartilhado)**: Contém classes base comuns, como a exceção de domínio base `DomainException` e o middleware global de tratamento de erros `ErrorHandlingMiddleware`.

---

## Estrutura do Projeto

```
src/
├── SharedKernel/             (Class Lib - Componentes Compartilhados)
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
## Configuração do Ambiente (.env)

Antes de executar o projeto, copie o arquivo de exemplo de ambiente `.env.example` na raiz do projeto para um novo arquivo chamado `.env`:
```bash
cp .env.example .env
```
Este arquivo `.env` contém as configurações de host, porta, credenciais e nome do banco de dados PostgreSQL. Ele é ignorado pelo Git para proteger credenciais e é carregado dinamicamente tanto pelo Docker Compose quanto localmente pela aplicação (através do componente `EnvLoader` no `SharedKernel`).

---

## Como Executar a Aplicação

### Opção A: Pelo Visual Studio (Recomendado)
1. Abra o arquivo de solução `TicketApi.sln` no **Visual Studio 2022** (ou superior).
2. O Visual Studio identificará o projeto de orquestração `docker-compose`.
3. Clique com o botão direito sobre o projeto `docker-compose` e selecione **"Definir como Projeto de Inicialização"** (Set as Startup Project).
4. Pressione **F5** (ou clique em **Iniciar**). O Visual Studio iniciará os contêineres do PostgreSQL e dos microservices, anexará os depuradores automaticamente e abrirá o navegador na página do Swagger da API de Eventos.

### Opção B: Pela Linha de Comando (CLI)
Certifique-se de que o Docker esteja em execução na sua máquina. Na raiz do projeto, execute:
```bash
docker-compose up --build
```

---

## Portas de Acesso e Swagger

Após iniciar o ambiente Docker, as APIs e o banco estarão disponíveis nas seguintes portas locais:
* **Events API**: [http://localhost:5001](http://localhost:5001)
  * Swagger UI: [http://localhost:5001/swagger](http://localhost:5001/swagger)
* **Sales API**: [http://localhost:5002](http://localhost:5002)
  * Swagger UI: [http://localhost:5002/swagger](http://localhost:5002/swagger)
* **PostgreSQL**: Porta local configurada no `.env` (porta padrão: `5432`). Os esquemas do banco são criados e isolados logicamente:
  * O microserviço de Eventos utiliza o esquema de banco `events`.
  * O microserviço de Vendas utiliza o esquema de banco `sales`.

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

### 2. Aplicar Migrations no Banco Local
Com a leitura automática do `.env` pelo componente `EnvLoader`, as credenciais e o host local do PostgreSQL são carregados automaticamente ao executar os comandos na sua máquina local (Host). Basta rodar os comandos diretamente:

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
