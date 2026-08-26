# BLA backend

.NET 10 task-management API built with the Kitchen Menu Clean Architecture flow: MediatR commands and queries, FluentValidation pipeline, endpoint groups, JWT bearer authentication, EF Core/PostgreSQL, health checks, and Scalar OpenAPI.

## Run locally

1. Copy `.env.example` to `.env` and set local-only passwords.
2. Run `docker compose up -d`.
3. Apply migrations with `dotnet ef database update --project src/Bla.Infrastructure --startup-project src/Bla.Api`.
4. Run `dotnet run --project src/Bla.Api`.

The API uses Keycloak realm `bla`. Registration is enabled; no user password or secret is committed. The React application should use `bla-web` with PKCE and request access tokens for audience `bla-api`.

## API

- `GET /v1/public/ping` is anonymous.
- `GET|POST /v1/tasks` and `GET|PUT|DELETE /v1/tasks/{id}` require a bearer JWT.
- Ownership is derived exclusively from the Keycloak `sub` claim. A request for another user's task returns `404`.

Run tests with `dotnet test tests/Bla.Application.Tests/Bla.Application.Tests.csproj`.
