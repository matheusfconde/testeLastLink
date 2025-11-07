# **🏗️ LastLink Products API**

API desenvolvida em **.NET 8** seguindo o padrão **Clean Architecture (Domain → Application → Infrastructure → API)**.

O sistema gerencia produtos, publica eventos de criação no **RabbitMQ** e salva logs de eventos no banco **PostgreSQL** por meio de um **Consumer** dedicado.

## **📚 Sumário**

* [Arquitetura]
* [Tecnologias]
* [Estrutura do Projeto]
* [Endpoints]
* [Execução com Docker]
* [Execução local 
* [Migrations e Banco de Dados]
* [Testes] 
* [Fluxo de Mensageria]
* [Evoluções Futuras (Produção)] 
* [Autor]

## **🧠 Arquitetura**

A API foi construída com base em **Clean Architecture**, garantindo separação de responsabilidades e facilitando a manutenção:

**Domain → Application → Infrastructure → API**

* **Domain**: Entidades e eventos de domínio (Product, ProductCreatedEvent).  
* **Application**: Interfaces e contratos de serviços e repositórios.  
* **Infrastructure**: Implementação da persistência (EF Core \+ PostgreSQL) e mensageria (RabbitMQ via MassTransit).  
* **API**: Endpoints RESTful (Minimal API em .NET 8).  
* **Consumer**: Serviço independente que escuta o RabbitMQ e grava logs de eventos no banco de dados.

## **🧰 Tecnologias**

| Categoria | Tecnologias |
| :---- | :---- |
| Linguagem | C\# / .NET 8 |
| Banco de Dados | PostgreSQL |
| ORM | Entity Framework Core |
| Mensageria | RabbitMQ \+ MassTransit |
| Containerização | Docker \+ Docker Compose |
| Testes | xUnit \+ Moq \+ EF InMemory |
| Padrões | Clean Architecture, DDD (Domain-Driven Design) |
| Cloud ready | AWS / Azure friendly |

## **🗂️ Estrutura do Projeto**

lastlink-products/  
├─ src/  
│ ├─ Domain/  
│ │ ├─ Entities/ → Product.cs, ProductEvent.cs  
│ │ └─ Events/ → ProductCreatedEvent.cs  
│ ├─ Application/  
│ │ ├─ Interfaces/ → IProductRepository.cs  
│ │ └─ DTOs/  
│ ├─ Infrastructure/  
│ │ ├─ Persistence/ → AppDbContext.cs  
│ │ ├─ Repositories/ → ProductRepository.cs  
│ ├─ Api/  
│ │ └─ Program.cs → Minimal API  
│ └─ Consumer/  
│   └─ Program.cs → Consumer do RabbitMQ  
│  
├─ tests/  
│ └─ lastlink.products.tests/ → xUnit Tests  
│  
├─ scripts/  
│ └─ init\_db.sql → Script de criação inicial de tabelas  
│  
├─ docker-compose.yml  
└─ README.md

## **🌐 Endpoints**

Base URL (local): http://localhost:5000

| Método | Endpoint | Descrição |
| :---- | :---- | :---- |
| **POST** | /products | Cria um novo produto e publica evento product.created no RabbitMQ |
| **GET** | /products | Retorna todos os produtos cadastrados |
| **GET** | /products/{id} | Busca produto por ID |
| **PUT** | /products/{id} | Atualiza um produto existente |
| **DELETE** | /products/{id} | Remove um produto |

💡 **Nota:** Ao criar um produto, um evento é publicado no RabbitMQ. O **consumer** escuta esse evento e grava um log na tabela ProductEvents.

## **🐳 Execução com Docker**

### **Pré-requisitos**

* Docker e Docker Compose instalados

### **Comandos**

\# Build e inicialização de todos os serviços (API, DB, RabbitMQ e Consumer)  
docker compose up \--build (na raiz do projeto /testeLastLink)

**Serviços disponíveis:**

| Serviço | Porta | Descrição |
| :---- | :---- | :---- |
| API | 5000 | Endpoints REST |
| PostgreSQL | 5432 (Local) / 5433 (Docker) | Banco de dados |
| RabbitMQ Broker | 5672 | Conexão para mensageria |
| RabbitMQ UI | 15672 | Interface Web de Gerenciamento |
| Consumer | — | Serviço background que processa eventos |

**Após o docker compose up, acesse:**

* **Swagger:** http://localhost:5000/swagger  
* **RabbitMQ UI:** http://localhost:15672 (login: guest / senha: guest)

## **💻 Execução local (sem Docker)**

### **Requisitos**

* .NET 8 SDK  
* PostgreSQL local (usuário postgres, senha postgres padrão)  
* RabbitMQ local (porta 5672\)

### **Comandos**

\# Restaurar dependências  
dotnet restore

\# Aplicar migrações  
dotnet ef database update \--project src/Infrastructure \--startup-project src/Api

\# Rodar API e Consumer (em terminais separados)  
dotnet run \--project src/Api  
dotnet run \--project src/Consumer

Acesse a API em http://localhost:5000.

## **🗃️ Migrations e Banco de Dados**

A base de dados é criada e atualizada automaticamente via Migrations do EF Core.

Se desejar recriar o banco manualmente, use o script:

psql \-U postgres \-d lastlink\_products \-f scripts/init\_db.sql

Tabelas principais: Products e ProductEvents.

## **🧪 Testes**

Os testes foram criados com xUnit, usando Moq para simular dependências e EF Core InMemory para simular o banco de dados.

### **Executar testes**

dotnet test

### **Cobertura**

Os testes cobrem as principais unidades:

* Entidades (Product, ProductEvent, ProductCreatedEvent)  
* Repositório (ProductRepository)  
* Consumer (Lógica de processamento do evento RabbitMQ)  
* Fluxo de criação de produto (mock de repositório e publisher)

## **🔄 Fluxo de Mensageria**

O fluxo assíncrono é o coração do sistema:

\[POST /products\] (API)  
       │  
       ▼  
  ProductRepository.AddAsync() (Grava no DB)  
       │  
       ▼  
 Publica evento → RabbitMQ (fila 'product.created')  
       │  
       ▼  
 Consumer lê da fila  
       │  
       ▼  
 Grava Log em tabela ProductEvents (Garantia de que o evento foi processado)

## **🚀 Evoluções Futuras (Produção)**

Esta seção demonstra pontos de melhoria e refinamento para um ambiente de produção real:

1. **Outbox Pattern:** Utilizar a estratégia Outbox Pattern para garantir a atomicidade na gravação do produto no banco e a publicação da mensagem na fila, evitando inconsistências em caso de falha.  
2. **Segurança de Credenciais:** As credenciais presentes no código (.NET) e no docker-compose.yml devem ser movidas para um cofre de segredos (Secret Manager, Azure Vault, AWS Secrets Manager), mantendo a segurança em produção.

## **👤 Autor**

Matheus Condé  
💻 Desenvolvedor Back-end (.NET, Python, Node.js, AWS)  
📧 contato: matheusf.conde@gmail.com  
🔗 LinkedIn: https://www.linkedin.com/in/mfconde/

## **🏁 Licença**

Este projeto é de uso técnico e educacional. Sinta-se à vontade para usar como referência para testes, estudos ou portfólio.
