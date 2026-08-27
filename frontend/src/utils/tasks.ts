import type { TaskItem, TaskStatus } from '../services/tasks'

export function getNextTaskStatus(status: TaskStatus): TaskStatus | null {
  if (status === 'Todo') return 'InProgress'
  if (status === 'InProgress') return 'Done'
  return null
}

export function getQuickActionLabel(status: TaskStatus): string | null {
  if (status === 'Todo') return 'Start task'
  if (status === 'InProgress') return 'Mark done'
  return null
}

export function getTaskStatusLabel(status: TaskStatus): string {
  if (status === 'Todo') return 'To do'
  if (status === 'InProgress') return 'In progress'
  return 'Done'
}

export function getTodayDate(now: Date = new Date()): string {
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')

  return `${year}-${month}-${day}`
}

export function isDueOnDate(dueDate: string, date: string): boolean {
  return dueDate.slice(0, 10) === date
}

export function isOverdue(dueDate: string, todayDate: string): boolean {
  return dueDate !== '' && dueDate.slice(0, 10) < todayDate
}

export function sortTasksByDueDate(tasks: readonly TaskItem[]): TaskItem[] {
  return [...tasks].sort((firstTask, secondTask) => {
    if (!firstTask.dueDate && !secondTask.dueDate) return 0
    if (!firstTask.dueDate) return 1
    if (!secondTask.dueDate) return -1
    return firstTask.dueDate.localeCompare(secondTask.dueDate)
  })
}
