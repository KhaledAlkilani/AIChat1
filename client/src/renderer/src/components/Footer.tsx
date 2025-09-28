import { getTokenString, getUserFromToken, Me } from '@renderer/auth/jwt'
import { Box, FormControl, IconButton, InputBase, Theme, useTheme } from '@mui/material'
import SendIcon from '@mui/icons-material/Send'
import { useEffect, useState } from 'react'
import { api } from '@renderer/api/api'

type FooterProps = { sessionId: string | null }

const Footer = ({ sessionId }: FooterProps) => {
  const theme = useTheme()
  const [text, setText] = useState('')
  const [me, setMe] = useState<Me | null>(null)

  // read token once when the footer mounts
  useEffect(() => {
    const t = getTokenString()
    setMe(t ? getUserFromToken(t) : null)
  }, [])

  const send = async () => {
    const content = text.trim()
    if (!content || !me || !sessionId) return
    try {
      await api.ChatService.sendMessage({ userId: me.id, content })
      setText('')
    } catch (err) {
      console.error('Failed to send:', err)
    }
  }

  const onKeyDown = async (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      await send()
    }
  }

  const disabled = !text.trim() || !me || !sessionId

  return (
    <Box sx={styles.footer(theme)}>
      <FormControl sx={{ width: '100%' }}>
        <InputBase
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={onKeyDown}
          sx={{ flex: 1, padding: theme.spacing(0.5) }}
          placeholder="Ask what’s on your mind"
          endAdornment={
            <IconButton onClick={send} aria-label="send" disabled={disabled}>
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
  footer: (theme: Theme) => ({
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(1),
    backgroundColor: theme.palette.background.paper,
    borderTop: `1px solid ${theme.palette.divider}`
  })
}
