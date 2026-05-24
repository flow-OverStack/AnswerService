# Flow OverStack – AnswerService

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_AnswerService&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_AnswerService)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_AnswerService&metric=coverage)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_AnswerService)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_AnswerService&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_AnswerService)

## Project Overview

AnswerService is a microservice in the Flow OverStack platform responsible for managing answers to questions, including
creation,
editing, deletion, acceptance, and voting. It handles answer-related business logic and publishes domain events through
the outbox.

The service authenticates requests with JWT tokens issued by Keycloak and communicates with UserService and
QuestionService over gRPC.

## 🚀 Quick Start a ready-made API

1. Install [Docker Desktop](https://www.docker.com/)
2. [Quick Start](https://github.com/flow-OverStack/UserService?tab=readme-ov-file#-quick-start-a-ready-made-api) the
   User Service.
3. [Quick Start](https://github.com/flow-OverStack/QuestionService?tab=readme-ov-file#-quick-start-a-ready-made-api) the
   Question Service.
4. Copy [the docker-compose.yaml](https://github.com/flow-OverStack/AnswerService/blob/master/docker-compose.yaml) file
   into one directory
5. Copy (and reconfigure if
   needed) [logstash.conf](https://github.com/flow-OverStack/AnswerService/blob/master/logstash.conf) file in the same
   directory
6. Create and configure `.env` file in the same directory:
   ```env
   ANSWER_DB_PASSWORD=db_password
   REDIS_PASSWORD=redis_password
   ```
7. On the first run (or after updating migrations), apply EF Core migrations to the database:

   **Option A — Automatic ✅ Recommended for Quick Start**

   Run `AnswerService.Api` with `ASPNETCORE_ENVIRONMENT=Development`.
   Migrations are applied automatically on startup.

   If you run the API in Docker, temporarily set `ASPNETCORE_ENVIRONMENT: Development` in the service environment:
   ```yaml
   answer-service:
      # ... other variables
      environment:
        # ... other variables
        ASPNETCORE_ENVIRONMENT: Development
   ```
   Start the service — migrations will be applied automatically on startup.
   > ⚠️ After the first run, **remove** `ASPNETCORE_ENVIRONMENT: Development` from the `docker-compose.yaml` file and
   restart the container.

   **Option B — Manual SQL script (Production)**

   Generate a SQL script with `dotnet ef migrations script` and apply it to the database
   manually ([Production approach](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#sql-scripts))

8. Start dependencies and the API:
    ```bash
   docker-compose -p answerservice -f docker-compose.yaml up -d
   cd AnswerService.Api
   dotnet run
   ```
9. Explore endpoints at `/swagger/v1/swagger.json`.

## Technologies and Patterns Used

* **.NET 10 & C#** — Core framework and language
* **ASP.NET Core** — HTTP API
* **Entity Framework Core with PostgreSQL** — Data access (Repository & Unit of Work patterns) to PostgreSQL database
* **Kafka** — Message queue that listens to main events
* **gRPC clients** — High-performance RPC calls to UserService and QuestionService
* **Redis** — Caching layer with short-lived entity caching and negative caching (null values caching)
* **Hot Chocolate** — GraphQL endpoint with built-in support for pagination, filtering, and sorting
* **Clean Architecture** — Layered separation (Domain, Application, Infrastructure, Presentation)
* **Outbox Pattern** — ensures reliable message delivery to the message queue
* **Decorator Pattern** — allows behavior to be added to individual objects dynamically without affecting others. In
  this project, it is used to implement caching.
* **Hangfire** — Hosted services for background jobs
* **Resilience** — Standard .NET resilience handler for HTTP clients (retries, circuit breaker, timeout), Hangfire
  retries and MassTransit retries, circuit breaker and kill switch
* **Observability** — Traces, logs, and metrics collected via OpenTelemetry and Logstash, exported to Aspire dashboard,
  Jaeger, ElasticSearch, and Prometheus
* **Monitoring & Visualization** — Dashboards in Grafana, Kibana, and Aspire
* **Health Checks** — Status endpoints to monitor service availability and dependencies
* **xUnit & Coverlet** — Automated unit and integration testing with code coverage
* **SonarQube** — Code quality and coverage analysis

## Architecture and Design

This service follows the principles of Clean Architecture. The solution is split into multiple projects that correspond
to each architectural layer.

![Clean Architecture](https://www.milanjovanovic.tech/blogs/mnw_017/clean_architecture.png?imwidth=1920)

| Layer              | Project                                                                                                                                       |
|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| **Presentation**   | AnswerService.Api, AnswerService.GraphQl                                                                                                      |
| **Application**    | AnswerService.Application                                                                                                                     |
| **Domain**         | AnswerService.Domain                                                                                                                          |
| **Infrastructure** | AnswerService.BackgroundJobs, AnswerService.Cache, AnswerService.DAL, AnswerService.GrpcClient, AnswerService.Messaging, AnswerService.Outbox |

Full system design on
Miro: [Application Structure Board](https://miro.com/app/board/uXjVLx6YYx4=/?share_link_id=993967197754)

## Getting Started for developers

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Docker Desktop](https://www.docker.com/)

### Installation

1. Clone the repo
2. Reconfigure if needed `appsettings.json` and `.NET User Secrets` in `AnswerService.Api` with your database, Redis,
   and gRPC hosts.
   `.NET User Secrets` example:
   ```json
   {
       "ConnectionStrings": {
           "PostgresSQL": "Server=localhost;Port=5436; Database=answer-service-db; User Id=<YOUR-USER-ID>; Password=<YOUR-PASSWORD>"
       },
       "RedisSettings": {
           "Password": "<YOUR-PASSWORD>"
       },
       "GrpcHosts": {
           "UsersHost": "http://localhost:8086",
           "QuestionsHost": "http://localhost:8088"
       }
   }
   ```
3. Start
   the [UserService](https://github.com/flow-OverStack/UserService/tree/master?tab=readme-ov-file#getting-started-for-developers)
   and
   [QuestionService](https://github.com/flow-OverStack/QuestionService/tree/master?tab=readme-ov-file#getting-started-for-developers)
   first,
   as AnswerService depends on them and common services (such as Kafka, Keycloak, etc.)
4. Start dependencies (you can use [Quick Start](#-quick-start-a-ready-made-api) or run your own services)
5. Run the API:

   ```bash
   cd AnswerService.Api
   dotnet run
   ```
   or use your IDE.

## API Documentation

The following endpoints are available by default:

| REST API & Swagger                             | GraphQL Endpoint               |
|------------------------------------------------|--------------------------------|
| https://localhost:7216/swagger/v1/swagger.json | https://localhost:7216/graphql |

## Testing

Run unit and functional tests:

```bash
cd AnswerService.Tests
dotnet test --filter Category=Functional
dotnet test --filter Category=Unit
```

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_AnswerService&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_AnswerService)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_AnswerService&metric=coverage)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_AnswerService)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=flow-OverStack_AnswerService&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=flow-OverStack_AnswerService)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to your branch
5. Open a Pull Request

Please follow the existing code conventions and include tests for new functionality.
You are also welcome to open issues for bug reports, feature requests, or to discuss improvements.

## License

This project is licensed under the MIT License. See
the [LICENSE](https://github.com/flow-OverStack/AnswerService/blob/master/LICENSE) file for details.

## Contact

For questions or suggestions open an issue.
