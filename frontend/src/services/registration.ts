export type RegistrationDraft = Readonly<{ username: string; email: string; displayName: string; password: string }>

export async function registerUser(draft: RegistrationDraft): Promise<void> {
  const response = await fetch(`${import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'}/v1/users`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(draft) })
  if (response.ok) return
  if (response.status === 409) throw new Error('Username or email already in use.')
  const problem = await response.json().catch(() => null) as { detail?: string; title?: string } | null
  throw new Error(problem?.detail ?? problem?.title ?? 'Could not create the account.')
}
