# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

AnswerService is one microservice of the **Flow OverStack** platform (a Stack Overflow-like system). It owns answers to questions: posting, editing, deleting, accepting/revoking acceptance, and voting (upvote/downvote/remove vote). It authenticates requests via JWT issued by **Keycloak**, fetches users and questions from **UserService** and **QuestionService** over **gRPC**, and publishes domain events to **Kafka** through a transactional **outbox**.

Target framework is **.NET 10** (see `global.json`). The repo uses sibling services UserService and QuestionService as runtime dependencies.

## Common commands

Run from the repo root unless noted.

```bash
# Build everything
dotnet build

# Run the API (Swagger UI + auto-migrate only in Development)
cd AnswerService.Api && dotnet run

# Tests are split by xUnit trait Category=Unit / Category=Functional
cd AnswerService.Tests
dotnet test --filter Category=Unit
dotnet test --filter Category=Functional

# Run a single test by name
dotnet test --filter "FullyQualifiedName~PostAnswerHandlerTests"
dotnet test --filter "Category=Unit&FullyQualifiedName~GetAnswers"

# EF Core migrations (DAL holds the DbContext, Api is the startup project)
dotnet ef migrations add <Name> -p AnswerService.DAL -s AnswerService.Api
dotnet ef migrations script -p AnswerService.DAL -s AnswerService.Api   # production schema apply

# Bring up infra dependencies (Postgres, Redis, Kafka, observability stack)
docker-compose -p answerservice -f docker-compose.yaml up -d
```

**Migrations apply automatically on startup only when `ASPNETCORE_ENVIRONMENT=Development`** (`Program.cs` calls `MigrateDatabaseAsync`). In production, apply the generated SQL script manually.

**Functional tests are infrastructure-heavy**: `FunctionalTestWebAppFactory` spins up real Postgres and Redis via **Testcontainers** (Docker must be running), stubs Keycloak with **WireMock**, and replaces the gRPC clients and Kafka producer with in-memory test doubles. No external services are needed to run them, but Docker is.

## Architecture

Clean Architecture, one project per concern. Dependencies point inward toward `Domain`.

| Layer | Projects |
|-------|----------|
| Presentation | `AnswerService.Api` (REST controllers, middleware, startup), `AnswerService.GraphQl` (Hot Chocolate read API) |
| Application | `AnswerService.Application` (MediatR commands/queries/handlers, validators, mappings) |
| Domain | `AnswerService.Domain` (entities, DTOs, interfaces, `BaseResult`/`CollectionResult`, business rules) |
| Infrastructure | `AnswerService.DAL`, `AnswerService.Cache`, `AnswerService.GrpcClient`, `AnswerService.Messaging`, `AnswerService.Outbox`, `AnswerService.BackgroundJobs` |

Composition root is `AnswerService.Api/Program.cs` + `Startup.cs`. Each infrastructure project exposes its own `AddX()` extension under `DependencyInjection/`.

### CQRS via MediatR

- **Writes** go through REST: `AnswerController` → `IMediator.Send(command)` → command handler in `Application/Handlers/`. Handlers return `BaseResult<T>`; the controller's `HandleBaseResult` maps success/`ErrorCode` to HTTP status.
- **Reads** go through GraphQL (`Queries.cs`) → `IMediator.Send(query)` → query handler returning `IQueryable<T>` (`QueryableResult`) or `CollectionResult<T>`. Hot Chocolate applies paging/filtering/sorting over the `IQueryable`.
- A MediatR `ValidationBehavior` pipeline runs FluentValidation validators before handlers. Registration is done by **reflection** in `Application/DependencyInjection/DependencyInjection.cs` — it scans the assembly and only wires the behavior for handlers whose response is `BaseResult<T>`. Validators are also auto-registered, including resolving `IValidator<TInterface>` to concrete types.

### Caching via decorators

Query handlers under `Application/Handlers/Get/` have **cache decorator counterparts** under `Application/Handlers/Decorators/Cache/Get/`. `AddCacheHandlerDecorators` reflectively decorates each inner handler with its cache handler (via Scrutor `Decorate`) when a matching non-decorator counterpart exists. The decorator hits Redis through `IAnswerCacheRepository` / `IVoteCacheRepository` / `IVoteTypeCacheRepository` (in `AnswerService.Cache`), falling back to the inner handler on a miss. Caching is **short-lived** and uses **negative caching** (caches null/not-found to absorb repeated misses). When adding a new cached query, add both the handler and its decorator in the namespace the scanner expects.

### Outbox → Kafka event flow

Write handlers that produce events (e.g. `AcceptAnswerHandler`, `UpvoteAnswerHandler`, `DownvoteAnswerHandler`, `RemoveVoteHandler`, `RevokeAcceptanceHandler`) call `IBaseEventProducer.ProduceAsync(...)`. Flow:

1. `BaseEventProducer` (Messaging) builds a `BaseEvent` and calls `IOutboxService.AddToOutboxAsync` → an `OutboxMessage` row is written to Postgres **in the same DbContext/transaction** as the business change.
2. `OutboxBackgroundService` (Hangfire-hosted) periodically calls `OutboxProcessor.ProcessOutboxMessagesAsync`, which reads unprocessed messages, resolves a `ITopicProducer` per event type via `ITopicProducerResolver`, and publishes to Kafka through **MassTransit**.
3. Failures are retried with a fixed back-off schedule (5s → 24h, 10 attempts); exhausted messages are marked dead. `OutboxResetJob` / `OutboxResetService` resets stuck messages.

`PostAnswerHandler`, `EditAnswerHandler`, `DeleteAnswerHandler` are CRUD-only and do not emit events.

### External data providers

`AnswerService.GrpcClient` implements `IEntityProvider<UserDto>` and `IEntityProvider<QuestionDto>` over gRPC (protos in `GrpcClient/Protos/`, hosts in `GrpcHosts` settings). Handlers depend on the `IEntityProvider<T>` abstraction, not gRPC directly. HTTP clients use the standard .NET resilience handler (retry/circuit-breaker/timeout). Functional tests swap these clients for `GrpcTestUserService` / `GrpcTestQuestionService`.

### Cross-cutting

- **Errors / i18n**: handlers return localized messages from `Application/Resources/ErrorMessage*.resx` (cultures `en`, `ru-by`) keyed by `ErrorCodes`. `ExceptionHandlingMiddleware` and (for GraphQL) `PublicErrorFilter` shape error responses; `WarningHandlingMiddleware` and `ClaimsValidationMiddleware` run in the pipeline.
- **Auditing**: `DateInterceptor` (DAL) stamps `CreatedAt`/`LastModifiedAt` on `IAuditable` entities. Repository + Unit of Work pattern in `DAL/Repositories/`.
- **Observability**: OpenTelemetry traces/metrics + Serilog logs exported to Aspire dashboard, Jaeger, Prometheus, Elasticsearch (via Logstash). Health checks at `/health`.

## Conventions

- Handlers return `BaseResult<T>` / `CollectionResult<T>` / `QueryableResult<T>` — do not throw for expected business failures; return a failure with an `ErrorCodes` value and a resource key.
- Answer body length rules live in `Domain/Settings/BusinessRules.cs` (min 30, max 30000).
- DI wiring relies on reflection + namespace conventions (validators, MediatR behaviors, cache decorators). New components must land in the expected namespaces/shapes or they will not be registered.
- Tag every test with `[Trait("Category", "Unit")]` or `"Functional"` so the CI filters pick it up.

## Notes

- `.github/workflows/`: `tests.yml` runs unit + functional tests with Coverlet (OpenCover), then triggers `sonarqube.yml` and `docker.yml`. Code quality/coverage is gated by SonarCloud.
- `AnswerSevice.GraphQl/` (note the misspelling) is a stale/duplicate project being removed — the live GraphQL project is `AnswerService.GraphQl/`. Don't add code to the misspelled one.
