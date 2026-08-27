import { useEffect, useRef, useState } from 'react'
import { UserIcon } from './Icons'

export function UserMenu({ username, onLogout }: Readonly<{ username: string; onLogout: () => void }>) {
  const [isOpen, setIsOpen] = useState(false)
  const menuRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!isOpen) return undefined

    function closeWhenClickedOutside(event: PointerEvent) {
      if (!menuRef.current?.contains(event.target as Node)) setIsOpen(false)
    }

    document.addEventListener('pointerdown', closeWhenClickedOutside)
    return () => document.removeEventListener('pointerdown', closeWhenClickedOutside)
  }, [isOpen])

  return <div className="user-menu" ref={menuRef}>
    <button className="user-menu-trigger" type="button" onClick={() => setIsOpen((current) => !current)} aria-label={`Open account menu for ${username}`} aria-expanded={isOpen} title="Account"><UserIcon /></button>
    {isOpen ? <div className="user-menu-popover"><button className="secondary" type="button" onClick={onLogout}>Log out</button></div> : null}
  </div>
}
