import { useEffect, useState } from 'react'

type Theme = 'dark' | 'light'

function getInitialTheme(): Theme {
  const savedTheme = window.localStorage.getItem('bla-theme')
  if (savedTheme === 'dark' || savedTheme === 'light') return savedTheme
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

export function useTheme(): { isDark: boolean; toggleTheme: () => void } {
  const [theme, setTheme] = useState<Theme>(getInitialTheme)

  useEffect(() => {
    document.body.dataset.theme = theme
    window.localStorage.setItem('bla-theme', theme)
  }, [theme])

  return { isDark: theme === 'dark', toggleTheme: () => setTheme((currentTheme) => currentTheme === 'dark' ? 'light' : 'dark') }
}
