import { MoonIcon, SunIcon } from './Icons'

export function ThemeToggle({ isDark, onToggle }: Readonly<{ isDark: boolean; onToggle: () => void }>) {
  return <button className="theme-toggle" type="button" onClick={onToggle} aria-label={isDark ? 'Use light mode' : 'Use dark mode'} title={isDark ? 'Use light mode' : 'Use dark mode'}>{isDark ? <SunIcon /> : <MoonIcon />}</button>
}
