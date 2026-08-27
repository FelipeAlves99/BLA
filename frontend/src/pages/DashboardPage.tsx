import { useState } from 'react'
import { Modal } from '../components/Modal'
import { TaskForm } from '../components/TaskForm'
import { TaskList } from '../components/TaskList'
import { ThemeToggle } from '../components/ThemeToggle'
import { UserMenu } from '../components/UserMenu'
import { useTheme } from '../hooks/useTheme'
import { useTodayDate } from '../hooks/useTodayDate'
import { useTasks } from '../hooks/useTasks'
import type { TaskItem } from '../services/tasks'
import { isDueOnDate, sortTasksByDueDate } from '../utils/tasks'

export function DashboardPage({ username, onLogout }: Readonly<{ username: string; onLogout: () => void }>) {
  const { advanceTask, deleteTaskById, error, isLoading, isSaving, loadTaskForEditing, saveTask, tasks } = useTasks()
  const { isDark, toggleTheme } = useTheme()
  const todayDate = useTodayDate()
  const [editingTask, setEditingTask] = useState<TaskItem | null>(null)
  const [isTaskModalOpen, setIsTaskModalOpen] = useState(false)
  const todaysTasks = sortTasksByDueDate(tasks.filter((task) => isDueOnDate(task.dueDate, todayDate)))
  const remainingTasks = sortTasksByDueDate(tasks.filter((task) => !isDueOnDate(task.dueDate, todayDate)))

  function closeModal() {
    setEditingTask(null)
    setIsTaskModalOpen(false)
  }

  function openCreateModal() {
    setEditingTask(null)
    setIsTaskModalOpen(true)
  }

  async function openEditModal(id: string) {
    const task = await loadTaskForEditing(id)
    if (task) {
      setEditingTask(task)
      setIsTaskModalOpen(true)
    }
  }

  async function saveModalTask(draft: Parameters<typeof saveTask>[1]) {
    if (await saveTask(editingTask, draft)) closeModal()
  }

  return <main className="dashboard">
    <header className="topbar">
      <h1 className="workspace-title">BLA Workspace</h1>
      <div className="header-actions"><ThemeToggle isDark={isDark} onToggle={toggleTheme} /><UserMenu username={username} onLogout={onLogout} /></div>
    </header>
    {error ? <p className="error-message" role="alert">{error}</p> : null}
    <section className="workspace" aria-label="Task management">
      <section className="card records-panel">
        {isLoading ? <p className="muted">Loading tasks…</p> : <>
          <section className="task-section" aria-labelledby="todays-tasks-heading">
            <div className="today-header"><h2 className="task-section-title" id="todays-tasks-heading">Today&apos;s tasks</h2><span>{todaysTasks.length} due today</span><button type="button" onClick={openCreateModal}>+ Add task</button></div>
            <TaskList tasks={todaysTasks} emptyMessage="No tasks are due today." isSaving={isSaving} onAdvance={advanceTask} onEdit={openEditModal} onDelete={deleteTaskById} todayDate={todayDate} />
          </section>
          <section className="task-section" aria-labelledby="remaining-tasks-heading">
            <div className="panel-heading"><h2 className="task-section-title" id="remaining-tasks-heading">Remaining tasks</h2><span>{remainingTasks.length} remaining</span></div>
            <TaskList tasks={remainingTasks} emptyMessage="No remaining tasks." isSaving={isSaving} onAdvance={advanceTask} onEdit={openEditModal} onDelete={deleteTaskById} todayDate={todayDate} />
          </section>
        </>}
      </section>
    </section>
    {isTaskModalOpen ? <Modal isDismissible={!isSaving} onClose={closeModal} titleId="task-modal-title"><TaskForm key={editingTask?.id ?? 'new'} defaultDueDate={todayDate} editingTask={editingTask} isSaving={isSaving} onSave={saveModalTask} onCancel={closeModal} /></Modal> : null}
  </main>
}
