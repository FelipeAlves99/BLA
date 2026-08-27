import { useEffect, useState } from 'react'
import { getTodayDate } from '../utils/tasks'

function millisecondsUntilNextLocalDay(now: Date): number {
  const nextDay = new Date(now)
  nextDay.setHours(24, 0, 0, 0)
  return nextDay.getTime() - now.getTime()
}

export function useTodayDate(): string {
  const [todayDate, setTodayDate] = useState(getTodayDate)

  useEffect(() => {
    const timeoutId = window.setTimeout(() => setTodayDate(getTodayDate()), millisecondsUntilNextLocalDay(new Date()))
    return () => window.clearTimeout(timeoutId)
  }, [todayDate])

  return todayDate
}
