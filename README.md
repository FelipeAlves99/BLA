# BLA

BLA is a task-management exercise with a .NET API, PostgreSQL, Keycloak, and a React + TypeScript client.

## Project structure

- [`backend/`](backend/) — .NET 10 API, database migrations, Docker services, and Keycloak realm configuration.
- [`frontend/`](frontend/) — React + TypeScript application built with Vite.

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

## Configure and run the backend

1. Create a local environment file from the example:

   ```powershell
   Copy-Item backend/.env.example backend/.env
   ```

2. Edit `backend/.env` and set values for `POSTGRES_PASSWORD` and `KEYCLOAK_ADMIN_PASSWORD`. Keep this file local; it must not be committed.

3. Start PostgreSQL and Keycloak:

   ```powershell
   Set-Location backend
   docker compose up -d
   ```

4. Apply the database migrations:

   ```powershell
   dotnet ef database update --project src/Bla.Infrastructure --startup-project src/Bla.Api
   ```

5. Start the API:

   ```powershell
   dotnet run --project src/Bla.Api
   ```

Keycloak is available at `http://localhost:8080`. The development realm is `bla`; its browser client is `bla-web` and the API audience is `bla-api`.

## Run the frontend

In a second terminal:

```powershell
Set-Location frontend
npm install
npm run dev
```

Open the URL shown by Vite, normally `http://localhost:5173`.

Useful frontend commands:

```powershell
npm run lint
npm run build
npm run preview
```

## Frontend configuration status

The current frontend is a small UI prototype. Its username/password login form is local-only and its CRUD records are kept in React state, so it runs without environment variables or a live backend. The credentials are not sent to or validated by Keycloak.

When API integration is added, configure the React client for Keycloak Authorization Code Flow with PKCE using client `bla-web`, and send its bearer access token to the API. The API task routes are under `/v1/tasks` and require that token.

## Test the backend

From `backend/`, run:

```powershell
dotnet test tests/Bla.Application.Tests/Bla.Application.Tests.csproj
dotnet test tests/Bla.Api.IntegrationTests/Bla.Api.IntegrationTests.csproj
dotnet build Bla.sln
```

Docker must be running for the integration tests.
