# BLA backend

.NET 10 task-management API built with the Kitchen Menu Clean Architecture flow: MediatR commands and queries, FluentValidation pipeline, endpoint groups, JWT bearer authentication, EF Core/PostgreSQL, health checks, and Scalar OpenAPI.

## Run locally

1. Copy `.env.example` to `.env` and set local-only database, Keycloak admin, and registration-client secrets.
2. Run `docker compose up -d`.
3. Apply migrations with `dotnet ef database update --project src/Bla.Infrastructure --startup-project src/Bla.Api`.
4. Run `dotnet run --project src/Bla.Api`.

The API uses Keycloak realm `bla`. Registration is enabled; no user password or secret is committed. The React application should use `bla-web` with PKCE and request access tokens for audience `bla-api`.

The anonymous `POST /v1/users` endpoint provisions a Keycloak identity through the confidential `bla-registration-api` service account, then creates the matching local `ApplicationUser`. Its secret is supplied as `KEYCLOAK_REGISTRATION_CLIENT_SECRET` in `.env`; the service account needs `realm-management` → `manage-users`. A duplicate username or email returns `409 Conflict`.

In Development only, the API seeds three sample tasks for the `demo` user when that user has no tasks. It never overwrites or adds to an existing demo task list.

## API

- `GET /v1/public/ping` is anonymous.
- `GET|POST /v1/tasks` and `GET|PUT|DELETE /v1/tasks/{id}` require a bearer JWT.
- Ownership is derived exclusively from the Keycloak `sub` claim. A request for another user's task returns `404`.
- `POST /v1/users` is anonymous and creates an account; browser login remains Keycloak PKCE.

Run tests with the Application, Infrastructure, and API integration test projects. The integration suite requires Docker/Testcontainers.
