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

The app provides a Create account form that calls the anonymous `/v1/users` API, then redirects users to Keycloak for sign-in using PKCE. Authenticated task calls use the received bearer token. Run `npm test` for component and utility tests, and `npm run build` for a production build.
