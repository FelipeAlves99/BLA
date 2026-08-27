export function LoginPage({ error, onLogin, onRegister }: Readonly<{ error: string | null; onLogin: () => void; onRegister: () => void }>) {
  return <main className="login-page">
    <section className="card login-card">
      <p className="eyebrow">BLA Workspace</p>
      <h1>Welcome back</h1>
      <p className="muted">Sign in with your BLA username and password to manage tasks.</p>
      {error ? <p className="error-message" role="alert">{error}</p> : null}
      <button type="button" onClick={onLogin}>Sign in with Keycloak</button>
      <button className="secondary" type="button" onClick={onRegister}>Create account</button>
    </section>
  </main>
}
