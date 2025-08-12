import { Box } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useEffect, useState } from 'react'
import { MessageDto } from '@renderer/api/api'
import { OpenAPI } from '@renderer/api'

const ChatMessages = () => {
  const theme = useTheme()
  const [msgs, setMsgs] = useState<MessageDto[]>([])

  useEffect(() => {
    const base = OpenAPI.BASE || 'http://localhost:5164'
    const conn = new HubConnectionBuilder()
      .withUrl(`${base}/chat`)
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build()

    conn.on('ReceiveMessage', (sender: string, text: string) => {
      setMsgs((prev) => [
        ...prev,
        {
          id: 0,
          userId: 0,
          username: sender,
          content: text,
          sentAt: new Date().toISOString()
        }
      ])
    })

    conn.start().catch(console.error)
    return () => {
      void conn.stop()
    }
  }, [])

  return (
    <Box sx={styles.ChatMessages(theme)}>
      {msgs.map((m, i) => (
        <Box key={i}>
          <strong>{(m as any).username ?? m.userId}:</strong> {m.content}
        </Box>
      ))}
    </Box>
  )
}

export default ChatMessages

const styles = {
  ChatMessages: (theme: Theme) => ({
    flexGrow: 1,
    p: 2,
    overflowY: 'auto',
    bgcolor: theme.palette.background.paper
  })
}
