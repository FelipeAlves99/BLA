# BLA repository guidelines

## Monorepo layout

- This repository is a monorepo.
- `backend/` contains the .NET API and all backend-specific instructions.
- `frontend/` is reserved for the React + TypeScript client.
- Do not place backend code in `frontend/` or frontend code in `backend/`.
- `backend/legacy-reference/` contains archived Kitchen Menu material. It is not part of the active solution; do not modify or build it unless explicitly asked.

## Repository-wide working rules

- Keep changes inside the relevant monorepo area and avoid unrelated refactors.
- Do not commit local environment files, generated artifacts, IDE settings, or real secrets.
- Follow the closest `AGENTS.md` file for area-specific rules. For files under `backend/`, `backend/AGENTS.md` supplements and takes precedence over this file.
