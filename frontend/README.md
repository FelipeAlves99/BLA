# BLA Frontend

React + TypeScript client for the BLA task API.

## Run locally

1. Start the backend, PostgreSQL, and Keycloak as described in the [root README](../README.md).
2. Copy `.env.example` to `.env`. The default values target the local API and Keycloak services.
3. Install and start the client:

   ```powershell
   npm install
   npm run dev
   ```

The app redirects to Keycloak for sign-in using PKCE, then calls the authenticated `/v1/tasks` API with the received bearer token.
