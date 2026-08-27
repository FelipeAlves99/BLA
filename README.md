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
   $env:ASPNETCORE_URLS = "http://localhost:5000"
   dotnet run --project src/Bla.Api
   ```

Keycloak is available at `http://localhost:8080`. The development realm is `bla`; its browser client is `bla-web` and the API audience is `bla-api`.

## Run the frontend

In a second terminal:

```powershell
Set-Location frontend
Copy-Item .env.example .env
npm install
npm run dev
```

Open the URL shown by Vite, normally `http://localhost:5173`.

The default `frontend/.env.example` configuration expects the API at `http://localhost:5000` and Keycloak at `http://localhost:8080`. Change `VITE_API_BASE_URL` only if you run the API on a different URL. Do not put passwords, client secrets, or access tokens in this file.

Useful frontend commands:

```powershell
npm run lint
npm run build
npm run preview
```

## Frontend authentication and API integration

The frontend redirects to Keycloak for username/password sign-in using Authorization Code Flow with PKCE. It does not collect or store the password itself. After sign-in, it sends the Keycloak bearer access token to the API and performs task CRUD operations through `/v1/tasks`.

The local realm includes development users `demo` / `demo` and `other` / `other` for testing only. Do not use these credentials outside local development.

## Test the backend

From `backend/`, run:

```powershell
dotnet test tests/Bla.Application.Tests/Bla.Application.Tests.csproj
dotnet test tests/Bla.Api.IntegrationTests/Bla.Api.IntegrationTests.csproj
dotnet build Bla.sln
```

Docker must be running for the integration tests.
