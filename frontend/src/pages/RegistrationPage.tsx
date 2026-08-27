import { useState } from 'react'
import type { FormEvent } from 'react'
import { registerUser } from '../services/registration'

export function RegistrationPage({ onBack }: Readonly<{ onBack: () => void }>) {
  const [error, setError] = useState<string | null>(null)
  const [isSaving, setIsSaving] = useState(false)
  const [isComplete, setIsComplete] = useState(false)
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setError(null); setIsSaving(true)
    const form = new FormData(event.currentTarget)
    try { await registerUser({ username: String(form.get('username')), email: String(form.get('email')), displayName: String(form.get('displayName')), password: String(form.get('password')) }); setIsComplete(true) } catch (reason) { setError(reason instanceof Error ? reason.message : 'Could not create the account.') } finally { setIsSaving(false) }
  }
  return <main className="login-page"><section className="card login-card">{isComplete ? <><h1>Account created</h1><p className="muted">You can now sign in with your new account.</p><button type="button" onClick={onBack}>Back to sign in</button></> : <form className="record-form" onSubmit={submit}><p className="eyebrow">BLA Workspace</p><h1>Create account</h1>{error ? <p className="error-message" role="alert">{error}</p> : null}<label htmlFor="username">Username</label><input id="username" name="username" required /><label htmlFor="email">Email</label><input id="email" name="email" type="email" required /><label htmlFor="displayName">Display name</label><input id="displayName" name="displayName" required /><label htmlFor="password">Password</label><input id="password" name="password" type="password" required /><div className="form-actions"><button type="submit" disabled={isSaving}>{isSaving ? 'Creating…' : 'Create account'}</button><button className="secondary" type="button" onClick={onBack}>Back</button></div></form>}</section></main>
}
