import { useState } from 'react'
import { useAuth } from './hooks/useAuth'
import { DashboardPage } from './pages/DashboardPage'
import { LoginPage } from './pages/LoginPage'
import { RegistrationPage } from './pages/RegistrationPage'

export default function App() {
  const { error, isAuthenticated, login, logout, username } = useAuth()
  const [isRegistering, setIsRegistering] = useState(false)

  if (isAuthenticated === null) {
    return <main className="login-page"><p className="muted">Checking your session…</p></main>
  }

  return isAuthenticated ? <DashboardPage username={username} onLogout={logout} /> : isRegistering ? <RegistrationPage onBack={() => setIsRegistering(false)} /> : <LoginPage error={error} onLogin={login} onRegister={() => setIsRegistering(true)} />
}
