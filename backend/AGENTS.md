# BLA backend guidelines

These rules apply to `backend/` and all nested files.

## Solution and architecture

- Open `Bla.sln` in Rider.
- The solution view groups production projects under `src` and tests under `tests`.
- The dependency direction is `Bla.Api -> Bla.Application -> Bla.Domain`; `Bla.Api` also depends on `Bla.Infrastructure`, which depends on Application and Domain.
- `Bla.Domain` contains framework-independent entities, enums, and business concepts only.
- `Bla.Application` contains commands, queries, handlers, validators, DTOs, and application interfaces.
- `Bla.Infrastructure` contains EF Core, PostgreSQL/Npgsql, Keycloak claim access, migrations, and dependency-injection implementations.
- `Bla.Api` contains minimal endpoint groups, HTTP mapping, authentication setup, exception handling, OpenAPI/Scalar, health checks, and host configuration.
- Do not introduce dependencies from Domain to ASP.NET Core, EF Core, PostgreSQL, Keycloak, or API types.

## Persistence

- Use only `Bla.Application.Common.Interfaces.IAppDbContext` for application persistence.
- `IAppDbContext` exposes `DbSet<ApplicationUser> Users`, `DbSet<TaskItem> Tasks`, and `SaveChangesAsync`.
- `Bla.Infrastructure.Persistence.AppDbContext` is its only implementation and is registered as `IAppDbContext` in `AddInfrastructure`.
- Application handlers may use EF Core async LINQ through `IAppDbContext`.
- Do not introduce repositories, unit-of-work wrappers, `ITaskRepository`, or `IUserProvisioner` without explicit approval.
- Keep EF mapping, Npgsql configuration, migrations, and database-specific behavior in Infrastructure.

## Task behavior and authorization

- `TaskItem` must retain `Id`, `OwnerId`, `Title`, `Description`, `Status`, `DueDate`, `CreatedAtUtc`, and `UpdatedAtUtc`.
- Derive task ownership exclusively from `ICurrentUser.Id`, which comes from the authenticated Keycloak JWT subject.
- Every get, update, and delete query must filter by both task ID and owner ID.
- A missing task and a task belonging to another user must both return `404 Not Found` without revealing ownership or existence.
- On first task creation, ensure that a matching `ApplicationUser` exists from token claims.

## Commands, validation, results, and HTTP

- Use MediatR commands for mutations and queries for reads.
- Place feature slices under `src/Bla.Application/Tasks/<Commands|Queries>/<UseCase>/`.
- Use FluentValidation. The MediatR validation behavior throws `FluentValidation.ValidationException`; the API handler converts it to `HttpValidationProblemDetails` with HTTP 400.
- Use `Bla.Domain.Common.Result` / `Result<T>` for expected handler failures.
- Map expected not-found/ownership failures to `ProblemDetails` with HTTP 404.
- Creation returns `201 Created` plus `Location`; deletion and successful update return `204 No Content`.
- Use minimal API endpoint groups that implement `IEndpointGroup`; `MapEndpoints()` discovers them.
- Use `/v1/...` for versioned API routes. `/v1/tasks` requires authorization; `/v1/public/ping`, `/healthz`, and `/readyz` remain anonymous.
- Bind request/command DTOs at the API boundary. Never return EF entities directly.

## Keycloak, configuration, and operations

- Authentication is Keycloak OIDC/OAuth 2.0 with bearer JWT validation.
- The React client uses Authorization Code Flow with PKCE via `bla-web`; the API audience is `bla-api`.
- Development authority: `http://localhost:8080/realms/bla`.
- `docker-compose.yml` starts PostgreSQL and Keycloak; `keycloak/import/bla-realm.json` defines the local realm.
- `demo` / `demo` and `other` / `other` are development-only users used to demonstrate resource isolation. Do not add real credentials or secrets to tracked files.
- Store local PostgreSQL and Keycloak bootstrap passwords in `backend/.env`; never commit that file.
- Run migrations explicitly. Never apply them automatically during API startup.
- Generate: `dotnet ef migrations add <Name> --project src/Bla.Infrastructure --startup-project src/Bla.Api`.
- Apply: `dotnet ef database update --project src/Bla.Infrastructure --startup-project src/Bla.Api`.

## Test conventions

- Name tests with `MethodName_Scenario_ExpectedBehavior`.
- Use explicit `// Arrange`, `// Act`, and `// Assert` sections in every test.
- Test one observable behavior per test.
- Unit tests belong in `tests/Bla.Application.Tests`; exercise handlers using EF Core InMemory through `IAppDbContext`, with no Docker, HTTP, Keycloak, filesystem, or real PostgreSQL.
- Integration tests belong in `tests/Bla.Api.IntegrationTests`; use `WebApplicationFactory<Program>` and `PostgreSqlContainer` from Testcontainers.
- Integration tests must run EF migrations and truncate `tasks` and `users` before each test. Never point them at a developer's local database.
- `TestAuthenticationHandler` is test-host-only. Production Keycloak/JWT setup must remain unchanged.
- Maintain coverage for validation, all CRUD operations, and cross-user ownership denial.

## Quality and commands

- Target .NET 10 with nullable reference types, implicit usings, warnings-as-errors, and the backend `.editorconfig`.
- Use cancellation tokens in async handlers, endpoints, and data access.
- Run before handoff:

```powershell
dotnet test tests/Bla.Application.Tests/Bla.Application.Tests.csproj
dotnet test tests/Bla.Api.IntegrationTests/Bla.Api.IntegrationTests.csproj
dotnet build Bla.sln
```

- Docker must be running for integration tests because Testcontainers starts isolated PostgreSQL.
