import { useCallback, useEffect, useState } from 'react'
import { createTask, deleteTask, getTask, listTasks, updateTask } from '../services/tasks'
import type { TaskDraft, TaskItem } from '../services/tasks'
import { getNextTaskStatus } from '../utils/tasks'

type TaskState = {
  error: string | null
  isLoading: boolean
  isSaving: boolean
  loadTaskForEditing: (id: string) => Promise<TaskItem | null>
  saveTask: (task: TaskItem | null, draft: TaskDraft) => Promise<boolean>
  tasks: readonly TaskItem[]
  deleteTaskById: (id: string) => Promise<void>
  advanceTask: (id: string) => Promise<void>
}

export function useTasks(): TaskState {
  const [tasks, setTasks] = useState<readonly TaskItem[]>([])
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  const refreshTasks = useCallback(async () => {
    try {
      setTasks(await listTasks())
      setError(null)
    } catch (caughtError) {
      setError(caughtError instanceof Error ? caughtError.message : 'Could not load tasks.')
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    let isCurrent = true

    void listTasks()
      .then((loadedTasks) => {
        if (isCurrent) {
          setTasks(loadedTasks)
          setError(null)
        }
      })
      .catch((caughtError: unknown) => {
        if (isCurrent) setError(caughtError instanceof Error ? caughtError.message : 'Could not load tasks.')
      })
      .finally(() => {
        if (isCurrent) setIsLoading(false)
      })

    return () => { isCurrent = false }
  }, [])

  async function saveTask(task: TaskItem | null, draft: TaskDraft): Promise<boolean> {
    setIsSaving(true)
    setError(null)
    try {
      if (task) await updateTask(task.id, draft)
      else await createTask(draft)
      await refreshTasks()
      return true
    } catch (caughtError) {
      setError(caughtError instanceof Error ? caughtError.message : 'Could not save the task.')
      return false
    } finally {
      setIsSaving(false)
    }
  }

  async function loadTaskForEditing(id: string): Promise<TaskItem | null> {
    setIsSaving(true)
    setError(null)
    try {
      return await getTask(id)
    } catch (caughtError) {
      setError(caughtError instanceof Error ? caughtError.message : 'Could not load the task.')
      return null
    } finally {
      setIsSaving(false)
    }
  }

  async function deleteTaskById(id: string): Promise<void> {
    setIsSaving(true)
    setError(null)
    try {
      await deleteTask(id)
      await refreshTasks()
    } catch (caughtError) {
      setError(caughtError instanceof Error ? caughtError.message : 'Could not delete the task.')
    } finally {
      setIsSaving(false)
    }
  }

  async function advanceTask(id: string): Promise<void> {
    setIsSaving(true)
    setError(null)
    try {
      const task = await getTask(id)
      const nextStatus = getNextTaskStatus(task.status)
      if (nextStatus) {
        await updateTask(id, { title: task.title, description: task.description, status: nextStatus, dueDate: task.dueDate || null })
        await refreshTasks()
      }
    } catch (caughtError) {
      setError(caughtError instanceof Error ? caughtError.message : 'Could not update the task status.')
    } finally {
      setIsSaving(false)
    }
  }

  return { error, isLoading, isSaving, loadTaskForEditing, saveTask, tasks, deleteTaskById, advanceTask }
}
