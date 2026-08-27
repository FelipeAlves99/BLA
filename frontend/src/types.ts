export type RecordItem = {
  id: number
  name: string
  description: string
}

export type RecordDraft = Omit<RecordItem, 'id'>
