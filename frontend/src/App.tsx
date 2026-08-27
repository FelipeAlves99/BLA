import { useAuth } from './hooks/useAuth'
import { DashboardPage } from './pages/DashboardPage'
import { LoginPage } from './pages/LoginPage'

export default function App() {
  const { error, isAuthenticated, login, logout, username } = useAuth()

  if (isAuthenticated === null) {
    return <main className="login-page"><p className="muted">Checking your session…</p></main>
  }

  return isAuthenticated ? <DashboardPage username={username} onLogout={logout} /> : <LoginPage error={error} onLogin={login} />
}
