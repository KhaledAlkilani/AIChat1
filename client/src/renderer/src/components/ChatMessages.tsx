import { Box, Typography } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { useEffect, useRef } from 'react'
import { MessageDto } from '@renderer/api/api'
import { OpenAPI } from '@renderer/api'
import { useChatMessages } from '../hooks/useChatMessages'

type Props = { sessionId: number | null }

const ChatMessages = ({ sessionId }: Props) => {
  const theme = useTheme()
  const {
    messages: msgs,
    setMessages: setMsgs,
    loading,
    error,
    refetchMessages
  } = useChatMessages(sessionId)
  // const [msgs, setMsgs] = useState<MessageDto[]>([])

  // const token = useMemo(() => localStorage.getItem('token') ?? '', [])
  const base =
    OpenAPI.BASE || (import.meta.env.VITE_API_BASE_URL as string) || 'http://localhost:5164'

  // const getToken = () => localStorage.getItem('token') ?? ''

  // autoscroll
  const endRef = useRef<HTMLDivElement | null>(null)
  useEffect(() => {
    endRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [msgs])

  useEffect(() => {
    // no connection if nothing to show yet
    // (keep if you want; otherwise remove this guard)
    // if (!sessionId) return;

    const conn = new HubConnectionBuilder()
      .withUrl(`${base}/chat`, {
        // IMPORTANT: read token fresh every time, do NOT capture it
        accessTokenFactory: () => localStorage.getItem('token') ?? ''
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build()

    const handler = (sender: string, text: string) => {
      const dto: MessageDto = {
        id: Date.now(),
        userId: 0,
        content: text,
        sentAt: new Date().toISOString(),
        username: sender as any
      } as any

      setMsgs((prev) => [...prev, dto])
    }

    conn.on('ReceiveMessage', handler)

    let cancelled = false
    const start = async () => {
      try {
        await conn.start()
      } catch (err) {
        // retry a bit later if start races during reload
        if (!cancelled) {
          setTimeout(start, 1500)
        }
        console.error(err)
      }
    }
    start()

    // when SignalR reconnected, refetch messages from backend
    conn.onreconnected(() => {
      console.log('SignalR reconnected, refetching messages...')
      refetchMessages()
    })

    return () => {
      cancelled = true
      conn.off('ReceiveMessage', handler)
      void conn.stop()
    }
  }, [base, sessionId, refetchMessages])

  return (
    <Box sx={styles.ChatMessages(theme)}>
      {loading && <Typography color="text.secondary">Loading messages...</Typography>}
      {error && <Typography color="error">Failed to load messages: {error.message}</Typography>}
      {msgs.map((m) => (
        <Box key={m.id} gap={1} sx={{ mb: 2, display: 'flex' }}>
          <strong>{(m as any).username ?? m.userId}:</strong> {m.content}
        </Box>
      ))}
      <div ref={endRef} />
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
