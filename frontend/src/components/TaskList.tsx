import { TrashIcon } from './Icons'
import type { TaskItem } from '../services/tasks'
import { getQuickActionLabel, getTaskStatusLabel, isOverdue } from '../utils/tasks'

export function TaskList({ emptyMessage, isSaving, onAdvance, onDelete, onEdit, tasks, todayDate }: Readonly<{
  emptyMessage: string
  isSaving: boolean
  onAdvance: (id: string) => Promise<void>
  onDelete: (id: string) => Promise<void>
  onEdit: (id: string) => Promise<void>
  tasks: readonly TaskItem[]
  todayDate: string
}>) {
  if (tasks.length === 0) return <p className="empty-state">{emptyMessage}</p>

  return <ul className="record-list">
    {tasks.map((task) => {
      const quickActionLabel = getQuickActionLabel(task.status)
      const isLate = task.status !== 'Done' && isOverdue(task.dueDate, todayDate)
      const statusClassName = `status-tag status-${task.status.toLowerCase()}`

      return <li className={`record${task.status === 'Done' ? ' record-done' : ''}`} key={task.id} onClick={() => { if (!isSaving) void onEdit(task.id) }}>
        <button className="task-card" type="button" onClick={(event) => { event.stopPropagation(); void onEdit(task.id) }} disabled={isSaving} aria-label={`Edit ${task.title}`}>
          <div className="task-card-header">
            <div className="task-card-statuses">
              <span className={statusClassName}>{getTaskStatusLabel(task.status)}</span>
              {isLate ? <span className="late-tag">Late</span> : null}
            </div>
            <div className="task-card-details">{task.dueDate ? <span className="due-date">Due {task.dueDate}</span> : null}</div>
          </div>
          <div className="task-meta"><h3>{task.title}</h3></div>
        </button>
        <div className="row-actions">
          {quickActionLabel ? <button className={task.status === 'InProgress' ? 'success' : undefined} type="button" onClick={(event) => { event.stopPropagation(); void onAdvance(task.id) }} disabled={isSaving}>{quickActionLabel}</button> : null}
          <button className="danger icon-button" type="button" onClick={(event) => { event.stopPropagation(); void onDelete(task.id) }} disabled={isSaving} aria-label={`Delete ${task.title}`} title="Delete task"><TrashIcon /></button>
        </div>
      </li>
    })}
  </ul>
}
