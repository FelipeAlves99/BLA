import { useState } from 'react'
import type { FormEvent } from 'react'
import type { TaskDraft, TaskItem, TaskStatus } from '../services/tasks'

export function TaskForm({ defaultDueDate, editingTask, isSaving, onCancel, onSave }: Readonly<{
  defaultDueDate: string
  editingTask: TaskItem | null
  isSaving: boolean
  onCancel: () => void
  onSave: (draft: TaskDraft) => Promise<void>
}>) {
  const isCompletedTask = editingTask?.status === 'Done'
  const [title, setTitle] = useState(editingTask?.title ?? '')
  const [description, setDescription] = useState(editingTask?.description ?? '')
  const [status, setStatus] = useState<TaskStatus>(editingTask?.status ?? 'Todo')
  const [dueDate, setDueDate] = useState(editingTask?.dueDate ?? defaultDueDate)

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    await onSave({ title: title.trim(), description: description.trim(), status, dueDate: dueDate || null })
  }

  return <form className="record-form" onSubmit={handleSubmit}>
    <h2 id="task-modal-title">{editingTask ? 'Edit task' : 'New task'}</h2>
    <label htmlFor="task-title">Title</label>
    <input id="task-title" value={title} onChange={(event) => setTitle(event.target.value)} disabled={isCompletedTask} required />
    <label htmlFor="task-description">Description</label>
    <textarea id="task-description" value={description} onChange={(event) => setDescription(event.target.value)} rows={4} />
    <label htmlFor="task-status">Status</label>
    <select id="task-status" value={status} onChange={(event) => setStatus(event.target.value as TaskStatus)} disabled={isCompletedTask}>
      <option value="Todo">To do</option>
      <option value="InProgress">In progress</option>
      <option value="Done">Done</option>
    </select>
    <label htmlFor="task-due-date">Due date</label>
    <input id="task-due-date" type="date" value={dueDate} onChange={(event) => setDueDate(event.target.value)} disabled={isCompletedTask} />
    {isCompletedTask ? <p className="muted">Completed tasks can only have their description updated.</p> : null}
    <div className="form-actions">
      <button type="submit" disabled={isSaving}>{isSaving ? 'Saving…' : editingTask ? 'Save changes' : 'Create task'}</button>
      <button className="secondary" type="button" onClick={onCancel} disabled={isSaving}>Cancel</button>
    </div>
  </form>
}
