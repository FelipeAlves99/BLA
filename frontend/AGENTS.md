# BLA frontend guidelines

These rules apply to `frontend/` and all nested files.

## Stack and commands

- Use React, TypeScript, Vite, Tailwind CSS v4, and Vitest with React Testing Library.
- Run `npm run lint`, `npm test`, and `npm run build` before handoff.
- Keep dependencies versioned deliberately; do not add UI libraries for small components already covered by the app's CSS.

## Structure and React rules

- `src/pages/` contains route-level screens only. `App.tsx` selects the authenticated screen; `main.tsx` bootstraps React Strict Mode.
- `src/components/` contains reusable visual components. Keep them controlled through props and callbacks; do not call APIs from presentation components.
- `src/hooks/` contains reusable stateful behavior such as authentication, task loading, local date handling, and theme selection.
- `src/services/` owns Keycloak and HTTP integration. API shapes are mapped to frontend types here.
- `src/utils/` contains deterministic, side-effect-free functions. Add unit tests next to utilities as `*.test.ts`.
- Keep components predictable and functional. Follow the Rules of React: call hooks unconditionally at the component top level, never mutate props or state, derive view data during render, and use effects only to synchronize with external systems.
- Prefer explicit `Readonly` prop types and immutable array operations. Do not introduce classes or mutable module state for UI state.

## API and authentication

- Authenticate through Keycloak Authorization Code Flow with PKCE only. Never collect, store, log, or commit passwords, tokens, or client secrets.
- Send authenticated task requests only through `services/tasks.ts`. The task API is `/v1/tasks` and its bearer token comes from `services/keycloak.ts`.
- A task list response may omit a description; fetch an individual task before opening its edit modal.
- Completed tasks are read-only except for description. Preserve this rule in both UI and backend behavior.

## Task dashboard design

- Use the local calendar date (`YYYY-MM-DD`), not a timestamp, for Today's tasks. A task is late only when its due date is before today and its status is not Done.
- Show Today's tasks and Remaining tasks separately. Sort each by due date and place undated tasks last.
- Cards are clickable to edit. Keep quick status action and delete controls distinct, stop their click propagation, and preserve accessible labels.
- Card layout: status and optional Late tags share the left side of the top row; due date remains at the far right; title is below. Done cards use a light green background.
- The quick action advances `To do -> In progress -> Done`; Mark done uses green. Delete uses the trash icon and the mobile action ratio is 4:1 (action:delete).
- Use the shared modal for both creation and editing. Clicking its backdrop closes it unless saving. New tasks default their due date to today.
- Keep the header as one responsive row: BLA Workspace on the left; theme toggle then user menu on the right. The logout menu dismisses on outside click.
- Maintain dark-mode parity for every new surface and preserve keyboard focus visibility.

## Styling and tests

- `src/assets/styles.css` is the global style entry and imports Tailwind with `@import "tailwindcss";`. Use existing semantic class names for this small app; do not mix arbitrary utility strings into the established component styling without a clear reason.
- Test behavior rather than implementation details. Cover date/status rules, forms, and user-visible interactive states. Keep tests deterministic and avoid calling Keycloak or the live API.
