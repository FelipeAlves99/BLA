import { useEffect, useState } from 'react'
import { getAuthenticatedUsername, initializeKeycloak, loginWithKeycloak, logoutFromKeycloak } from '../services/keycloak'

type AuthState = {
  error: string | null
  isAuthenticated: boolean | null
  login: () => void
  logout: () => void
  username: string
}

export function useAuth(): AuthState {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let isCurrent = true

    void initializeKeycloak()
      .then((authenticated) => {
        if (isCurrent) setIsAuthenticated(authenticated)
      })
      .catch(() => {
        if (isCurrent) {
          setError('Could not connect to Keycloak. Check that it is running and configured.')
          setIsAuthenticated(false)
        }
      })

    return () => { isCurrent = false }
  }, [])

  return {
    error,
    isAuthenticated,
    login: () => { void loginWithKeycloak() },
    logout: () => { void logoutFromKeycloak() },
    username: getAuthenticatedUsername(),
  }
}
