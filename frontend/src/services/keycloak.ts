import Keycloak from 'keycloak-js'

const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL ?? 'http://localhost:8080',
  realm: import.meta.env.VITE_KEYCLOAK_REALM ?? 'bla',
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID ?? 'bla-web',
})

let initialization: Promise<boolean> | undefined

export function initializeKeycloak(): Promise<boolean> {
  initialization ??= keycloak.init({
    onLoad: 'check-sso',
    pkceMethod: 'S256',
    checkLoginIframe: false,
  })

  return initialization
}

export function loginWithKeycloak(): Promise<void> {
  return keycloak.login({ redirectUri: window.location.origin })
}

export function logoutFromKeycloak(): Promise<void> {
  return keycloak.logout({ redirectUri: window.location.origin })
}

export async function getAccessToken(): Promise<string> {
  await keycloak.updateToken(30)

  if (!keycloak.token) {
    throw new Error('Your session has expired. Please sign in again.')
  }

  return keycloak.token
}

export function getAuthenticatedUsername(): string {
  return keycloak.tokenParsed?.preferred_username ?? 'Signed-in user'
}
