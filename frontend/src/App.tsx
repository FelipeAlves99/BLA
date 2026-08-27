import { useState } from 'react'
import type { FormEvent } from 'react'
import type { RecordDraft, RecordItem } from './types'

const initialRecords: readonly RecordItem[] = [
  { id: 1, name: 'Welcome record', description: 'Use this screen to manage your records.' },
]

function LoginPage({ onLogin }: Readonly<{ onLogin: (username: string) => void }>) {
  const [username, setUsername] = useState('')

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    onLogin(username.trim())
  }

  return (
    <main className="login-page">
      <form className="card login-card" onSubmit={handleSubmit}>
        <p className="eyebrow">BLA Workspace</p>
        <h1>Welcome back</h1>
        <p className="muted">Sign in to manage your records.</p>
        <label htmlFor="username">Username</label>
        <input id="username" value={username} onChange={(event) => setUsername(event.target.value)} placeholder="Enter your username" required />
        <label htmlFor="password">Password</label>
        <input id="password" type="password" placeholder="Enter your password" required />
        <button type="submit">Sign in</button>
      </form>
    </main>
  )
}

function RecordForm({ editingRecord, onSave, onCancel }: Readonly<{
  editingRecord: RecordItem | null
  onSave: (draft: RecordDraft) => void
  onCancel: () => void
}>) {
  const [name, setName] = useState(editingRecord?.name ?? '')
  const [description, setDescription] = useState(editingRecord?.description ?? '')

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    onSave({ name: name.trim(), description: description.trim() })
    setName('')
    setDescription('')
  }

  return (
    <form className="record-form" onSubmit={handleSubmit}>
      <h2>{editingRecord ? 'Edit record' : 'New record'}</h2>
      <label htmlFor="record-name">Name</label>
      <input id="record-name" value={name} onChange={(event) => setName(event.target.value)} required />
      <label htmlFor="record-description">Description</label>
      <textarea id="record-description" value={description} onChange={(event) => setDescription(event.target.value)} rows={4} required />
      <div className="form-actions">
        <button type="submit">{editingRecord ? 'Save changes' : 'Create record'}</button>
        {editingRecord ? <button className="secondary" type="button" onClick={onCancel}>Cancel</button> : null}
      </div>
    </form>
  )
}

function RecordList({ records, onEdit, onDelete }: Readonly<{
  records: readonly RecordItem[]
  onEdit: (record: RecordItem) => void
  onDelete: (id: number) => void
}>) {
  if (records.length === 0) {
    return <p className="empty-state">No records yet. Create your first one using the form.</p>
  }

  return (
    <ul className="record-list">
      {records.map((record) => (
        <li className="record" key={record.id}>
          <div>
            <h3>{record.name}</h3>
            <p>{record.description}</p>
          </div>
          <div className="row-actions">
            <button className="secondary" type="button" onClick={() => onEdit(record)}>Edit</button>
            <button className="danger" type="button" onClick={() => onDelete(record.id)}>Delete</button>
          </div>
        </li>
      ))}
    </ul>
  )
}

function Dashboard({ username, onLogout }: Readonly<{ username: string; onLogout: () => void }>) {
  const [records, setRecords] = useState<readonly RecordItem[]>(initialRecords)
  const [editingRecord, setEditingRecord] = useState<RecordItem | null>(null)

  function handleSave(draft: RecordDraft) {
    if (editingRecord) {
      setRecords((currentRecords) => currentRecords.map((record) => (
        record.id === editingRecord.id ? { ...record, ...draft } : record
      )))
      setEditingRecord(null)
      return
    }

    setRecords((currentRecords) => [...currentRecords, { id: Date.now(), ...draft }])
  }

  function handleDelete(id: number) {
    setRecords((currentRecords) => currentRecords.filter((record) => record.id !== id))
    setEditingRecord((currentRecord) => currentRecord?.id === id ? null : currentRecord)
  }

  return (
    <main className="dashboard">
      <header className="topbar">
        <div><p className="eyebrow">BLA Workspace</p><h1>Records</h1></div>
        <div className="account"><span>{username}</span><button className="secondary" type="button" onClick={onLogout}>Log out</button></div>
      </header>
      <section className="workspace" aria-label="Record management">
        <aside className="card"><RecordForm key={editingRecord?.id ?? 'new'} editingRecord={editingRecord} onSave={handleSave} onCancel={() => setEditingRecord(null)} /></aside>
        <section className="card records-panel"><div className="panel-heading"><h2>All records</h2><span>{records.length} total</span></div><RecordList records={records} onEdit={setEditingRecord} onDelete={handleDelete} /></section>
      </section>
    </main>
  )
}

export default function App() {
  const [username, setUsername] = useState<string | null>(null)

  return username ? <Dashboard username={username} onLogout={() => setUsername(null)} /> : <LoginPage onLogin={setUsername} />
}
