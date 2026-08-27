# BLA

BLA is a task-management exercise with a .NET API, PostgreSQL, Keycloak, and a React + TypeScript client.

## User story

As an authenticated workspace user, I want to create, view, update, and delete my own tasks, with a title, description, status, and optional due date, so that I can manage today's work and see what remains without exposing my tasks to other users.

The full acceptance criteria and implementation/presentation guide are in [`docs/IMPLEMENTATION_GUIDE.md`](docs/IMPLEMENTATION_GUIDE.md).

## Project structure

- [`backend/`](backend/) — .NET 10 API, database migrations, Docker services, and Keycloak realm configuration.
- [`frontend/`](frontend/) — React + TypeScript application built with Vite.
- [`docs/`](docs/) — implementation guide and GenAI development record for the interview presentation.

## Prerequisites

Install the following before running the full stack:

- Node.js 24 LTS (including npm)
- .NET 10 SDK
- Docker Desktop, running locally

Verify the installations:

```powershell
node --version
npm --version
dotnet --version
docker --version
```

## Run the full stack

1. Create a local environment file from the example:

   ```powershell
   Copy-Item backend/.env.example backend/.env
   ```

2. Edit `backend/.env` and set values for `POSTGRES_PASSWORD`, `KEYCLOAK_ADMIN_PASSWORD`, and `KEYCLOAK_REGISTRATION_CLIENT_SECRET`. Keep this file local; it must not be committed.

   `KEYCLOAK_REGISTRATION_CLIENT_SECRET` belongs to the confidential `bla-registration-api` client in the `bla` realm. Its service account requires the `realm-management` → `manage-users` role. This lets the API create accounts without exposing Keycloak administrative credentials to the browser.

3. Install the root workflow dependency and frontend dependencies:

   ```powershell
   npm install
   npm --prefix frontend install
   ```

4. Start PostgreSQL, Keycloak, database migrations, the API, and the frontend together:

   ```powershell
   npm run dev
   ```

Open the Vite URL shown in the terminal, normally `http://localhost:5173`. The API runs at `http://localhost:5000` and Keycloak at `http://localhost:8080`.

Use `Ctrl+C` to stop the API and frontend. PostgreSQL and Keycloak remain running so local data is preserved. Stop those services explicitly with:

```powershell
npm run services:down
```

The development realm is `bla`; its browser client is `bla-web` and the API audience is `bla-api`.

## Run an individual part

For frontend-only work, with the backend stack already running:

```powershell
Set-Location frontend
Copy-Item .env.example .env
npm install
npm run dev
```

The default `frontend/.env.example` configuration expects the API at `http://localhost:5000` and Keycloak at `http://localhost:8080`. Change `VITE_API_BASE_URL` only if you run the API on a different URL. Do not put passwords, client secrets, or access tokens in this file.

Useful frontend commands:

```powershell
npm run lint
npm run build
npm run preview
```

## Frontend authentication and API integration

The frontend redirects to Keycloak for username/password sign-in using Authorization Code Flow with PKCE. It does not collect or store the password itself. After sign-in, it sends the Keycloak bearer access token to the API and performs task CRUD operations through `/v1/tasks`.

Account registration is exposed by the anonymous `POST /v1/users` endpoint. The API validates the requested username, email, display name, and password, provisions the identity through Keycloak's Admin API, then stores the matching application user profile locally. A duplicate account returns `409 Conflict`.

The local realm includes development users `demo` / `demo` and `other` / `other` for testing only. On development startup, three sample tasks are seeded for `demo` only when that user has no tasks. Do not use these credentials outside local development.

## Test the backend

From `backend/`, run:

```powershell
dotnet test tests/Bla.Application.Tests/Bla.Application.Tests.csproj
dotnet test tests/Bla.Infrastructure.Tests/Bla.Infrastructure.Tests.csproj
dotnet test tests/Bla.Api.IntegrationTests/Bla.Api.IntegrationTests.csproj
dotnet build Bla.sln
```

Docker must be running for the integration tests.

## Root workflow commands

| Command | Purpose |
| --- | --- |
| `npm run dev` | Starts PostgreSQL and Keycloak, applies migrations, then runs the API and frontend. |
| `npm run services:up` | Starts only PostgreSQL and Keycloak. |
| `npm run services:down` | Stops the Docker services while preserving the PostgreSQL volume. |
| `npm run db:migrate` | Applies the EF Core migrations. |
