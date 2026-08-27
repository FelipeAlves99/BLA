import type { ReactNode } from 'react'

export function Modal({ children, isDismissible, onClose, titleId }: Readonly<{
  children: ReactNode
  isDismissible: boolean
  onClose: () => void
  titleId: string
}>) {
  return <div className="modal-backdrop" onMouseDown={(event) => { if (event.target === event.currentTarget && isDismissible) onClose() }}>
    <section className="card task-modal" role="dialog" aria-modal="true" aria-labelledby={titleId}>{children}</section>
  </div>
}
