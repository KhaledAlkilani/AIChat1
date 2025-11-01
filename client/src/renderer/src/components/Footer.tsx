import { useEffect, useState } from 'react'
import { AuthUser, getTokenString, getUserFromToken } from '@renderer/auth/jwt'
import { api } from '@renderer/api/api'
import { Box, FormControl, IconButton, InputBase, Theme, useTheme } from '@mui/material'
import SendIcon from '@mui/icons-material/Send'

interface FooterProps {
  sessionId: number | undefined
  onMessageSent: (firstUserText?: string, sessionIdArg?: number) => void
}

const Footer = ({ sessionId, onMessageSent }: FooterProps) => {
  const theme = useTheme()
  const [text, setText] = useState('')
  const [currentUser, setCurrentUser] = useState<AuthUser | null>(null)

  // Retrieve user info from token on mount
  useEffect(() => {
    const token = getTokenString()
    const user = token ? getUserFromToken(token) : null
    setCurrentUser(user)
  }, [])

  const handleSend = async () => {
    const content = text.trim()
    if (!content || !currentUser || !sessionId) {
    }
    setText('')

    try {
      // bind the message to the active conversation
      await api.ChatService.sendMessage({
        userId: currentUser?.id,
        content,
        ...({ conversationId: sessionId } as any)
      })

      setText('')
      onMessageSent(content, sessionId)
    } catch (err) {
      console.error('Failed to send message:', err)
    }
  }

  const handleKeyDown = async (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (text === '') {
      return
    }

    if (e.key === 'Enter' && !e.shiftKey) {
      setText('')
      e.preventDefault()
      await handleSend()
    }
  }

  const isDisabled = !text.trim() || !currentUser

  return (
    <Box sx={styles.footerContainer(theme)}>
      <FormControl sx={styles.formControl}>
        <InputBase
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Ask what’s on your mind"
          sx={styles.inputBase(theme)}
          endAdornment={
            <IconButton onClick={handleSend} aria-label="send" disabled={isDisabled}>
              <SendIcon />
            </IconButton>
          }
        />
      </FormControl>
    </Box>
  )
}

export default Footer

const styles = {
  footerContainer: (theme: Theme) => ({
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(1),
    backgroundColor: theme.palette.background.paper,
    borderTop: `1px solid ${theme.palette.divider}`
  }),
  formControl: {
    width: '100%'
  },
  inputBase: (theme: Theme) => ({
    flex: 1,
    padding: theme.spacing(0.5)
  })
}
