import { describe, expect, it } from 'vitest'
import type { TaskItem } from '../services/tasks'
import { getNextTaskStatus, getTodayDate, isDueOnDate, isOverdue, sortTasksByDueDate } from './tasks'

describe('task utilities', () => {
  it('getTodayDate_UsesTheLocalCalendarDate_ReturnsIsoDate', () => {
    expect(getTodayDate(new Date(2026, 7, 27, 23, 59))).toBe('2026-08-27')
  })

  it('isDueOnDate_DueDateIncludesTime_UsesOnlyTheCalendarDate', () => {
    expect(isDueOnDate('2026-08-27T23:59:59Z', '2026-08-27')).toBe(true)
  })

  it('isOverdue_TaskIsDueBeforeToday_ReturnsTrue', () => {
    expect(isOverdue('2026-08-26', '2026-08-27')).toBe(true)
    expect(isOverdue('2026-08-27', '2026-08-27')).toBe(false)
  })

  it('getNextTaskStatus_StatusIsDone_ReturnsNull', () => {
    expect(getNextTaskStatus('Done')).toBeNull()
  })

  it('sortTasksByDueDate_TasksHaveDates_ReturnsSortedCopyWithUndatedTasksLast', () => {
    const tasks: readonly TaskItem[] = [
      { id: 'later', title: 'Later', description: '', status: 'Todo', dueDate: '2026-08-29' },
      { id: 'undated', title: 'Undated', description: '', status: 'Todo', dueDate: '' },
      { id: 'earlier', title: 'Earlier', description: '', status: 'Todo', dueDate: '2026-08-27' },
    ]

    const sortedTasks = sortTasksByDueDate(tasks)

    expect(sortedTasks.map((task) => task.id)).toEqual(['earlier', 'later', 'undated'])
    expect(tasks.map((task) => task.id)).toEqual(['later', 'undated', 'earlier'])
  })
})
