import { getAccessToken } from './keycloak'

export type TaskStatus = 'Todo' | 'InProgress' | 'Done'

export type TaskItem = {
  id: string
  title: string
  description: string
  status: TaskStatus
  dueDate: string
}

export type TaskDraft = {
  title: string
  description: string
  status: TaskStatus
  dueDate: string | null
}

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

type ApiTask = {
  id: string
  title: string
  description?: string | null
  status: TaskStatus
  dueDate?: string | null
}

function toTaskItem(task: ApiTask): TaskItem {
  return {
    id: task.id,
    title: task.title,
    description: task.description ?? '',
    status: task.status,
    dueDate: task.dueDate?.slice(0, 10) ?? '',
  }
}

async function request(path: string, options: RequestInit = {}): Promise<Response> {
  const token = await getAccessToken()
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
      ...options.headers,
    },
  })

  if (!response.ok) {
    const detail = await response.json().catch(() => null) as { detail?: string; title?: string } | null
    throw new Error(detail?.detail ?? detail?.title ?? `Request failed with status ${response.status}.`)
  }

  return response
}

export async function listTasks(): Promise<readonly TaskItem[]> {
  const response = await request('/v1/tasks/')
  return (await response.json() as ApiTask[]).map(toTaskItem)
}

export async function getTask(id: string): Promise<TaskItem> {
  const response = await request(`/v1/tasks/${id}`)
  return toTaskItem(await response.json() as ApiTask)
}

export async function createTask(draft: TaskDraft): Promise<void> {
  await request('/v1/tasks/', { method: 'POST', body: JSON.stringify(draft) })
}

export async function updateTask(id: string, draft: TaskDraft): Promise<void> {
  await request(`/v1/tasks/${id}`, { method: 'PUT', body: JSON.stringify(draft) })
}

export async function deleteTask(id: string): Promise<void> {
  await request(`/v1/tasks/${id}`, { method: 'DELETE' })
}
