# GenAI development record

This record summarizes how AI assistance was used during development and how its suggestions were reviewed. It is intended as a concise presentation aid, not as a substitute for reading and understanding the code.

## Prompt summary

The work was carried out in small, reviewable steps. The prompts were equivalent to the following requests:

1. Build a task-management API using .NET, Clean Architecture, MediatR, FluentValidation, PostgreSQL, and ownership derived from the authenticated user.
2. Build a responsive React and TypeScript frontend with predictable functional components that follows the Rules of React and supports task CRUD.
3. Integrate the frontend with the authenticated API using Keycloak Authorization Code Flow with PKCE.
4. Refine the dashboard interaction and layout: group tasks by local due date, add task-state quick actions, support modal editing, responsive behavior, dark mode, and accessible controls.
5. Add focused frontend and backend tests, then document how to run and maintain the project.

The detailed implementation prompt used for backend feature work was:

```text
Implement task CRUD in a .NET clean-architecture solution. Keep the Domain framework-independent.
Place commands, queries, handlers, and validators in feature slices; use only IAppDbContext for
persistence; derive the task owner from ICurrentUser; and return the same not-found result for
missing and other-user tasks. Use FluentValidation and minimal API endpoints under /v1/tasks.
Return 201 with Location for creation and 204 for successful update/delete. Add focused unit and
integration tests. Inspect the existing conventions before changing files.
```

## Representative output

The generated scaffold was refined into feature-oriented query handlers. For example, the list handler returns a response DTO rather than an EF Core entity and applies ownership filtering before projection:

```csharp
return await db.Tasks
    .AsNoTracking()
    .Where(task => task.OwnerId == currentUser.Id)
    .OrderBy(task => task.CreatedAtUtc)
    .Select(task => new ListTasksResponse(
        task.Id,
        task.Title,
        task.Status,
        task.DueDate))
    .ToListAsync(ct);
```

See [ListTasksQueryHandler.cs](../backend/src/Bla.Application/Tasks/Queries/ListTasks/ListTasksQueryHandler.cs) and [GetTaskResponse.cs](../backend/src/Bla.Application/Tasks/Queries/GetTask/GetTaskResponse.cs) for the complete implementation.

## Review and corrections

AI output was treated as a starting point and corrected where it did not match the product or repository rules.

### Persistence boundary instead of a redundant repository pattern

The repository pattern was considered, but a generic task repository would only duplicate EF Core queries and obscure the application's small, explicit use cases. The project instead uses `IAppDbContext` as the Application-layer persistence port, with `AppDbContext` as its Infrastructure implementation. This keeps handlers testable and preserves a clear dependency direction without adding a wrapper that provides no additional domain behavior.

### Result and response DTOs

Handlers do not expose EF Core entities at the HTTP boundary. Query handlers project to `ListTasksResponse` and `GetTaskResponse`; expected failures use `Result` / `Result<T>` and are mapped to `ProblemDetails` by the API layer. This makes the list response intentionally lightweight while the edit flow fetches the full task before opening the modal.

### Naming and feature conventions

- Backend code follows feature slices such as `Tasks/Commands/CreateTask` and `Tasks/Queries/GetTask`.
- Commands and queries use explicit request/response names; domain `TaskItem` is distinguished from frontend `TaskItem` types by project boundaries and imports.
- React components use PascalCase, hooks use the `use` prefix, API integration is kept in `services/`, and pure task calculations live in `utils/`.
- Tests use `MethodName_Scenario_ExpectedBehavior` on the backend and describe observable behavior on the frontend.

### Security and business-rule fixes

- Task ownership is always derived from the authenticated identity, never from a client-supplied user ID.
- EF Core applies an owner-based global query filter to tasks; explicit owner predicates remain on single-task mutations and reads as defense in depth.
- Read, update, and delete lookups apply both task ID and owner ID, returning `404` for another user's task to avoid resource enumeration.
- Keycloak owns browser sign-in through PKCE; the React application never handles a password or stores a client secret.
- Completed tasks can update only their description. The frontend disables other fields and the backend enforces the same rule.
- Due-date grouping uses the local `YYYY-MM-DD` calendar date rather than timestamp comparison, preventing time-zone-related placement errors.

### UI and structure fixes

- The frontend was reorganized into `assets`, `components`, `hooks`, `pages`, `services`, and `utils`, separating presentational components from stateful and external-system code.
- Task creation and editing use one dismissible modal; task cards are clickable while quick-action and delete controls stop click propagation.
- The dashboard separates Today's tasks from Remaining tasks, labels overdue incomplete tasks, gives completed cards a light-green treatment, and remains usable on narrow screens.
- The header keeps theme and user controls together, supports a dismissible logout menu, and includes dark-mode styling.

## Validation performed

- Reviewed dependency direction and ensured framework/database details do not enter the Domain layer.
- Added and ran frontend linting, unit/component tests, and a production build.
- Added backend unit and API integration tests for CRUD, validation, anonymous access, public access, and cross-user ownership denial.
- Reviewed task API status codes, DTO projection, `ProblemDetails` mapping, and authenticated Keycloak integration.

The Application unit-test suite and frontend test suite pass in the local environment. The integration suite requires Docker because it starts an isolated PostgreSQL instance through Testcontainers; it should be rerun when Docker is available.

## Human ownership

AI accelerated scaffolding, test ideas, and UI iteration. The final architecture, security boundaries, validation rules, product behavior, and verification decisions were reviewed and owned by the developer.
