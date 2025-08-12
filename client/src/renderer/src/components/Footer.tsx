import { Box, FormControl, IconButton, InputBase } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'
import { useState } from 'react'
import { api } from '@renderer/api/api'
import SendIcon from '@mui/icons-material/Send'
import { OpenAPI } from '@renderer/api'

const Footer = () => {
  const theme = useTheme()
  const [text, setText] = useState('')
  const currentUserId = 1 // TODO: get this from your auth/user store

  const send = async () => {
    const content = text.trim()
    if (!content) return
    try {
      console.log('[send] POST', `${OpenAPI.BASE}/api/chat/send`, {
        userId: currentUserId,
        content
      })
      await api.ChatService.sendMessage({ userId: currentUserId, content })
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

  return (
    <Box sx={styles.footer(theme)}>
      <FormControl sx={{ width: '100%' }}>
        <InputBase
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={onKeyDown}
          sx={{ flex: 1, padding: theme.spacing(0.5) }}
          fullWidth
          placeholder="Ask what’s on your mind"
          endAdornment={
            <IconButton onClick={send} aria-label="send" disabled={!text.trim()}>
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
    gap: 1,
    padding: `${theme.spacing(2)} ${theme.spacing(1)}`,
    backgroundColor: theme.palette.grey[100],
    borderTop: `1px solid ${theme.palette.divider}`,
    color: theme.palette.text.secondary
  })
}
