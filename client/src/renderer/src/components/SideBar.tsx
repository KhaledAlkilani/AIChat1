import {
  Box,
  Button,
  List,
  ListItemButton,
  ListItemText,
  Typography,
  IconButton
} from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'
import DeleteIcon from '@mui/icons-material/Delete'
import { useNavigate } from 'react-router-dom'
import { logout } from '@renderer/auth/jwt'
import { api, ConversationDto } from '@renderer/api/api'

interface SideBarProps {
  active: number | null
  sessions: ConversationDto[]
  userId: number | null
  onSelect: (id: number | null) => void
  onCreate: () => void
  setSessions: React.Dispatch<React.SetStateAction<ConversationDto[]>>
}

const SideBar = ({ active, onSelect, onCreate, sessions, userId, setSessions }: SideBarProps) => {
  const theme = useTheme()
  const navigate = useNavigate()
  // const { sessions, userId, setSessions } = useChatSessions(true)

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this conversation?')) return
    if (userId == null) return

    try {
      await api.ChatService.deleteConversation(id)

      setSessions((prevSessions) => prevSessions.filter((s) => s.id !== id)) // 3. If active session was deleted, clear it

      if (active === id) {
        onSelect(null)
      }
    } catch (err) {
      console.error('Failed to delete conversation:', err)
    }
  }

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <Box sx={styles.sidebar(theme)}>
      <Box sx={{ display: 'flex', justifyContent: 'center' }}>
        <Typography variant="subtitle1" sx={styles.title}>
          Conversations
        </Typography>
      </Box>

      <Box sx={styles.scrollArea}>
        <List dense sx={styles.list}>
          {sessions.map((s) => {
            if (s.id == null) {
              return null
            }
            return (
              <ListItemButton key={s.id} selected={s.id === active} onClick={() => onSelect(s.id!)}>
                <ListItemText
                  primary={s.title ?? 'New chat'}
                  secondary={s.createdAt ? new Date(s.createdAt).toLocaleString() : ''}
                />
                <IconButton
                  size="small"
                  edge="end"
                  aria-label="delete"
                  onClick={(e) => {
                    e.stopPropagation()
                    handleDelete(s.id!)
                  }}
                >
                  <DeleteIcon fontSize="small" />
                </IconButton>
              </ListItemButton>
            )
          })}
        </List>
      </Box>

      <Box sx={styles.stickyButtons}>
        <Button
          variant="contained"
          size="small"
          fullWidth
          onClick={onCreate}
          sx={styles.newChatButton}
        >
          New Chat
        </Button>
        <Button
          variant="text"
          size="small"
          fullWidth
          color="error"
          onClick={handleLogout}
          sx={styles.logoutButton}
        >
          Logout
        </Button>
      </Box>
    </Box>
  )
}

export default SideBar

const styles = {
  sidebar: (theme: Theme) => ({
    minWidth: 280,
    width: '20%',
    borderRight: `1px solid ${theme.palette.divider}`,
    backgroundColor: theme.palette.grey[200],
    display: 'flex',
    flexDirection: 'column',
    pt: theme.spacing(2)
  }),
  title: {
    marginBottom: (theme) => theme.spacing(1)
  },
  scrollArea: {
    flexGrow: 1,
    overflowY: 'auto',
    marginBottom: 0,
    height: '100%'
  },
  stickyButtons: {
    display: 'flex',
    flexDirection: 'column',
    p: 2
  },
  list: {
    bgcolor: 'transparent'
  },
  newChatButton: {
    marginBottom: 1,
    textTransform: 'none'
  },
  logoutButton: {
    textTransform: 'none',
    alignSelf: 'flex-start'
  }
}
