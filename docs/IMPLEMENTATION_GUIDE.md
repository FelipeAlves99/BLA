# BLA Task Management - Implementation Guide

This guide turns the technical exercise into an implementation and presentation plan for the code currently in this repository. It is written for a backend developer and a frontend developer working in parallel, with AI coding assistance used deliberately and reviewed critically.

## User story

**As an authenticated workspace user, I want to create, view, update, and delete my own tasks, with a title, description, status, and optional due date, so that I can manage today's work and see what remains without exposing my tasks to other users.**

### Acceptance criteria

- A signed-in user can create, list, view, update, and delete only their own tasks.
- A task contains `title`, `description`, `status`, and optional `dueDate`.
- API attempts to read, update, or delete another user's task return `404 Not Found`.
- The UI supports those operations, is responsive, and presents tasks due today separately from other tasks.
- Data persists in PostgreSQL and a user record is created from the authenticated identity when that user creates a first task.

## Exercise requirements mapped to the current project

| Exercise requirement | Current implementation | Status | Follow-up evidence/task |
| --- | --- | --- | --- |
| Database with an application table and a user table | PostgreSQL tables `tasks` and `users`, EF Core mapping and migration | Implemented | Apply the migration and demonstrate data in the database. |
| REST API CRUD with appropriate verbs and results | `GET`, `POST`, `PUT`, and `DELETE` under `/v1/tasks` | Implemented | Verify responses in Scalar or a browser/API client. |
| User creation, login, authorized and anonymous endpoints | Keycloak authentication, first-create user provisioning, public ping, protected task routes | Implemented | Demonstrate Keycloak login and an anonymous `401` for tasks. |
| Data-access layer | `IAppDbContext` in Application, `AppDbContext` in Infrastructure, EF Core/Npgsql | Implemented | Explain why handlers depend on the interface, not `AppDbContext`. |
| Independent business logic and validation | Domain entities plus MediatR handlers and FluentValidation | Implemented | Show validation and the completed-task update rule. |
| Unit and API testing | Application unit tests and Testcontainers-backed integration tests | Implemented | Run the full test suite and address any failures. |
| Responsive CRUD frontend | React, TypeScript, Vite, Tailwind, Keycloak PKCE, dashboard and modal form | Implemented | Run lint, tests, and production build; manually check mobile/dark mode. |
| README and demo credentials/seed data | Root README, Keycloak realm import, development users | Implemented | Keep `.env` local and use only development credentials. |
| GenAI prompt, generated-code sample, and validation narrative | Not yet captured as a dedicated artefact | To do | Use the AI-assistance record below. |
| Presentation and code-review readiness | Not yet captured as a dedicated artefact | To do | Prepare the demo and review checklist below. |

## Architecture at a glance

```text
React + TypeScript (frontend/)
  -> Keycloak Authorization Code Flow + PKCE
  -> Bearer JWT
ASP.NET Core minimal API (Bla.Api)
  -> MediatR commands and queries (Bla.Application)
  -> Framework-independent TaskItem/ApplicationUser (Bla.Domain)
  -> EF Core + Npgsql/PostgreSQL, CurrentUser from JWT (Bla.Infrastructure)
```

The dependency direction is intentionally inward: `Bla.Api -> Bla.Application -> Bla.Domain`. `Bla.Infrastructure` implements the Application interfaces and is composed by the API. This keeps task rules and handlers independent of HTTP and PostgreSQL.

## Work plan

Tasks marked complete describe what is already present in the working tree. Do not reimplement them unless a test or review reveals a defect.

### 1. Establish the local environment - joint

- [ ] Install Node.js 24 LTS, .NET 10 SDK, and Docker Desktop.
- [ ] Copy `backend/.env.example` to `backend/.env`, set local PostgreSQL and Keycloak admin passwords, and keep the file untracked.
- [ ] Run `docker compose up -d` from `backend/`.
- [ ] Apply the existing migration:

  ```powershell
  dotnet ef database update --project src/Bla.Infrastructure --startup-project src/Bla.Api
  ```

- [ ] Run the API on `http://localhost:5000` and the frontend through Vite.
- [ ] Log in with the development account described in the root README; do not use those credentials outside local development.

Done when the dashboard loads an authenticated empty task list and `/v1/public/ping` returns `200`.

### 2. Backend domain and persistence - backend

- [x] Define `TaskItem` with ID, owner ID, title, description, status, due date, and audit dates in `backend/src/Bla.Domain/Tasks/TaskItem.cs`.
- [x] Define `ApplicationUser` in `backend/src/Bla.Domain/Identity/ApplicationUser.cs`.
- [x] Keep domain types free of ASP.NET Core, EF Core, Keycloak, and database dependencies.
- [x] Expose `Users`, `Tasks`, and `SaveChangesAsync` through `IAppDbContext`.
- [x] Map `users` and `tasks`, including task field limits, a `date` due-date column, owner foreign key, and task query index in Infrastructure.
- [x] Maintain migrations explicitly; never apply them automatically on API startup.
- [ ] If the schema changes, create and apply a new migration, then inspect the generated change before committing it:

  ```powershell
  dotnet ef migrations add <MeaningfulName> --project src/Bla.Infrastructure --startup-project src/Bla.Api
  dotnet ef database update --project src/Bla.Infrastructure --startup-project src/Bla.Api
  ```

### 3. Backend application behavior - backend

- [x] Use MediatR commands for create, update, and delete; use queries for list and get.
- [x] Validate create and update input with FluentValidation and convert invalid input to HTTP `400` validation details.
- [x] Derive ownership only from `ICurrentUser.Id`, backed by the Keycloak JWT `sub` claim.
- [x] Filter every single-task read, update, and delete by both task ID and owner ID.
- [x] Return the same `404` for missing tasks and other users' tasks.
- [x] Create the matching local `ApplicationUser` from token claims when a user first creates a task.
- [x] Return `201 Created` with `Location` from create and `204 No Content` from successful update/delete.
- [x] Enforce the product rule that a completed task may change only its description.
- [ ] Review every new handler for cancellation-token propagation and for accidental exposure of EF entities at the API boundary.

### 4. Backend HTTP, auth, and operations - backend

- [x] Map protected routes under `/v1/tasks` and keep `/v1/public/ping`, `/healthz`, and `/readyz` anonymous.
- [x] Configure JWT bearer validation for the local Keycloak realm; API audience is `bla-api`.
- [x] Provide OpenAPI/Scalar in development and `ProblemDetails` error responses.
- [x] Provide PostgreSQL and Keycloak through `backend/docker-compose.yml` plus a realm import.
- [ ] Manually verify this API contract after starting the stack:

  | Method | Route | Expected result |
  | --- | --- | --- |
  | `GET` | `/v1/public/ping` | `200` without a token |
  | `GET` | `/v1/tasks/` | `401` without a token, owned tasks with a token |
  | `POST` | `/v1/tasks/` | `201`, ID response, and `Location` |
  | `GET` | `/v1/tasks/{id}` | `200` for owner; `404` for a different user |
  | `PUT` | `/v1/tasks/{id}` | `204` for owner; `400` for invalid input; `404` for a different user |
  | `DELETE` | `/v1/tasks/{id}` | `204` for owner; `404` for a different user |

### 5. Backend tests - backend

- [x] Cover task creation, read, list, update, deletion, validation, and ownership in Application tests.
- [x] Cover CRUD, anonymous access, public ping, and cross-user denial with API integration tests using Testcontainers PostgreSQL.
- [ ] Add or adjust tests before changing behavior. Test one observable outcome per test and retain Arrange/Act/Assert comments.
- [ ] Run and record the result of:

  ```powershell
  Set-Location backend
  dotnet test tests/Bla.Application.Tests/Bla.Application.Tests.csproj
  dotnet test tests/Bla.Api.IntegrationTests/Bla.Api.IntegrationTests.csproj
  dotnet build Bla.sln
  ```

Docker must be running for the integration suite.

### 6. Frontend integration and behavior - frontend

- [x] Authenticate through Keycloak Authorization Code Flow with PKCE; do not collect or store passwords in the app.
- [x] Centralize authenticated task calls in `frontend/src/services/tasks.ts`.
- [x] Map API nullable fields to stable UI values and load one task before opening its edit form.
- [x] Implement create/edit in a shared modal, quick status progression, and delete controls.
- [x] Group tasks into Today and Remaining using local calendar dates, sort by due date, and place undated tasks last.
- [x] Make completed cards visually distinct and prevent edits other than description.
- [x] Provide theme switching, responsive header/action layouts, accessible labels, modal focus behavior, and error feedback.
- [ ] Manually confirm that card clicks edit, while status/delete clicks do not also open the edit modal.
- [ ] Check a narrow viewport and both color themes. Verify visible keyboard focus and that the user menu closes when clicking outside it.

### 7. Frontend tests and build - frontend

- [x] Cover task utility date/status rules and form behavior with Vitest and React Testing Library.
- [ ] Add a deterministic test whenever a user-visible interaction or date/status rule changes.
- [ ] Run:

  ```powershell
  Set-Location frontend
  npm run lint
  npm test
  npm run build
  ```

### 8. AI-assisted development record - joint

The exercise asks for the prompt, representative output, and critical evaluation. Keep this section accurate to work actually performed; do not claim that an AI tool wrote code it did not write.

Suggested scaffolding prompt:

```text
In this .NET 10 clean-architecture solution, implement task CRUD through MediatR.
Keep Domain framework-independent. Add commands/queries/validators under
Bla.Application/Tasks, use only IAppDbContext for persistence, derive the task owner
from ICurrentUser, and make missing/other-user task access return the same not-found
result. Use FluentValidation and minimal API endpoints under /v1/tasks. Return 201
with Location for creation and 204 for successful update/delete. Add focused unit and
integration tests. Before changing files, inspect the existing solution conventions.
```

- [ ] Save the exact prompts actually used, with tool/version and date.
- [ ] Include a small representative diff or code excerpt, not an unreviewed code dump.
- [ ] State the checks used to validate output: architecture/dependency review, tests, build, API contract checks, and manual UI flow.
- [ ] State corrections made after review, especially ownership filtering, validation, null/date mapping, completed-task behavior, and security boundaries.
- [ ] Explain the human decision: AI accelerated scaffolding and test ideas, but developers owned requirements interpretation, security decisions, and final verification.

### 9. Interview demonstration and code review - joint

- [ ] Start with the user story and trace it from login to persistence.
- [ ] Demonstrate: log in, create a task, edit it, advance its status, update completed-task description, delete a task, and log out.
- [ ] Demonstrate isolation: use the `other` development user and show that another user's task is not available.
- [ ] Show the database schema/migration, a handler, an endpoint, the frontend service, and representative tests.
- [ ] Be ready to explain Clean Architecture boundaries, why `IAppDbContext` is used, why `404` avoids resource enumeration, why PKCE is used, and why due dates are `DateOnly`/local dates.
- [ ] Show clean `git status`, passing validation commands, and no browser-console errors.

## Suggested implementation order for a new feature

1. Refine the user-visible acceptance criterion and API contract.
2. Add a failing Application test, then implement the Domain/Application behavior.
3. Add/adjust EF mapping and create a migration only when persistence changes.
4. Map the handler through a minimal API endpoint and add integration coverage.
5. Extend the frontend API type/service, then hook/state, then presentational components.
6. Add frontend tests, run all relevant checks, and manually validate the authenticated flow.
7. Record the human review of any AI-generated contribution.

## Definition of done

A change is ready when it preserves the architecture rules, has proportionate unit/API/UI tests, passes backend and frontend validation commands, keeps secrets out of source control, works through Keycloak with real local services, respects task ownership, and is reflected in the demonstration narrative where relevant.
