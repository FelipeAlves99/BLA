import { render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import type { TaskItem } from '../services/tasks'
import { TaskForm } from './TaskForm'

const onSave = vi.fn(async () => undefined)
const onCancel = vi.fn()

describe('TaskForm', () => {
  it('newTask_DefaultDueDateProvided_PopulatesTheDueDateInput', () => {
    render(<TaskForm defaultDueDate="2026-08-27" editingTask={null} isSaving={false} onCancel={onCancel} onSave={onSave} />)

    expect(screen.getByLabelText('Due date')).toHaveValue('2026-08-27')
  })

  it('completedTask_Editing_DisablesTaskFieldsButAllowsDescription', () => {
    const completedTask: TaskItem = {
      id: 'completed-task', title: 'Completed task', description: 'Original description', status: 'Done', dueDate: '2026-08-26',
    }

    render(<TaskForm defaultDueDate="2026-08-27" editingTask={completedTask} isSaving={false} onCancel={onCancel} onSave={onSave} />)

    expect(screen.getByLabelText('Title')).toBeDisabled()
    expect(screen.getByLabelText('Status')).toBeDisabled()
    expect(screen.getByLabelText('Due date')).toBeDisabled()
    expect(screen.getByLabelText('Description')).toBeEnabled()
    expect(screen.getByLabelText('Due date')).toHaveValue('2026-08-26')
  })
})
