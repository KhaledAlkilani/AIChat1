import { Box, Button, List, ListItemButton, ListItemText, Typography } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'
import { useEffect, useState } from 'react'
import { IconButton } from '@mui/material'
import DeleteIcon from '@mui/icons-material/Delete'

type Session = { id: string; title: string; createdAt: number; updatedAt: number }
type Props = { active: string | null; onSelect: (id: string | null) => void; onCreate: () => void }

const SideBar = ({ active, onSelect, onCreate }: Props) => {
  const theme = useTheme()
  const [sessions, setSessions] = useState<Session[]>(() => {
    try {
      return JSON.parse(localStorage.getItem('chat.sessions') || '[]')
    } catch {
      return []
    }
  })

  // poll for changes written by ChatMessages
  useEffect(() => {
    const i = setInterval(() => {
      try {
        setSessions(JSON.parse(localStorage.getItem('chat.sessions') || '[]'))
      } catch {
        setSessions([])
      }
    }, 800)
    return () => clearInterval(i)
  }, [])

  const handleDelete = (id: string) => {
    // optional confirm
    if (!confirm('Delete this conversation?')) return

    // 1) remove messages for this session
    localStorage.removeItem(`chat.messages:${id}`)

    // 2) remove from sessions list
    const sessionsRaw = localStorage.getItem('chat.sessions') || '[]'
    const sessionsArr = JSON.parse(sessionsRaw) as Session[]
    const updated = sessionsArr.filter((s) => s.id !== id)
    localStorage.setItem('chat.sessions', JSON.stringify(updated))

    // 3) clear "last active" if it was this one
    const last = localStorage.getItem('chat.active')
    if (last === id) localStorage.removeItem('chat.active')

    // 4) reflect in UI
    setSessions(updated)
    if (active === id) onSelect(null)
  }

  return (
    <Box sx={styles.sidebar(theme)}>
      <Typography color="black" variant="subtitle1" sx={{ mb: 1 }}>
        Conversations
      </Typography>
      <List dense sx={{ bgcolor: 'transparent', flex: 1 }}>
        {sessions.map((s) => (
          <ListItemButton key={s.id} selected={s.id === active} onClick={() => onSelect(s.id)}>
            <ListItemText
              primary={s.title || 'New chat'}
              // IMPORTANT: show createdAt (fixed), not updatedAt
              secondary={new Date(s.createdAt).toLocaleString()}
            />
            <IconButton
              size="small"
              edge="end"
              aria-label="delete"
              onClick={(e) => {
                e.stopPropagation() // don’t select the row
                handleDelete(s.id)
              }}
            >
              <DeleteIcon fontSize="small" />
            </IconButton>
          </ListItemButton>
        ))}
      </List>
      <Button variant="outlined" size="small" onClick={onCreate} sx={{ mb: 1 }}>
        New chat
      </Button>
    </Box>
  )
}

export default SideBar

const styles = {
  sidebar: (theme: Theme) => ({
    // FIXED WIDTH — no more resize
    width: 280,
    minWidth: 280,
    maxWidth: 280,
    flex: '0 0 280px',
    flexShrink: 0,

    height: '100%',
    boxSizing: 'border-box',
    borderRight: `1px solid ${theme.palette.divider}`,
    bgcolor: theme.palette.grey[200],
    p: 2,
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden' // keep internals from expanding the box
  })
}
