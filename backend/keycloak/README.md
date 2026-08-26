# BLA Keycloak setup

`docker compose up` imports `import/bla-realm.json`. It enables self-registration and provides the `bla-web` PKCE client plus the `bla-api` bearer audience. Configure local bootstrap credentials only in `backend/.env`; no credentials are versioned.
