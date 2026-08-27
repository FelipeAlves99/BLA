import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import { afterEach, describe, expect, it, vi } from 'vitest'
import { RegistrationPage } from './RegistrationPage'

function renderPage() {
  const onBack = vi.fn()
  render(<RegistrationPage onBack={onBack} />)
  fireEvent.change(screen.getByLabelText('Username'), { target: { value: 'new.user' } })
  fireEvent.change(screen.getByLabelText('Email'), { target: { value: 'new@bla.local' } })
  fireEvent.change(screen.getByLabelText('Display name'), { target: { value: 'New User' } })
  fireEvent.change(screen.getByLabelText('Password'), { target: { value: 'password' } })
  return onBack
}

afterEach(() => vi.unstubAllGlobals())

describe('RegistrationPage', () => {
  it('validRegistration_ApiCreatesAccount_ShowsSuccess', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(null, { status: 201 })))
    renderPage()
    fireEvent.click(screen.getByRole('button', { name: 'Create account' }))
    expect(await screen.findByRole('heading', { name: 'Account created' })).toBeInTheDocument()
  })

  it('duplicateRegistration_ApiReturnsConflict_ShowsClearMessage', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(null, { status: 409 })))
    renderPage()
    fireEvent.click(screen.getByRole('button', { name: 'Create account' }))
    expect(await screen.findByRole('alert')).toHaveTextContent('Username or email already in use.')
  })

  it('registration_ApiReturnsProblem_ShowsApiMessage', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(new Response(JSON.stringify({ detail: 'Registration is unavailable.' }), { status: 502, headers: { 'Content-Type': 'application/json' } })))
    renderPage()
    fireEvent.click(screen.getByRole('button', { name: 'Create account' }))
    await waitFor(() => expect(screen.getByRole('alert')).toHaveTextContent('Registration is unavailable.'))
  })
})
