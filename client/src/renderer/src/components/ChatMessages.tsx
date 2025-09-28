import { Box } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'
import { HubConnectionBuilder, LogLevel, HttpTransportType } from '@microsoft/signalr'
import { useEffect, useMemo, useRef, useState } from 'react'
import { MessageDto } from '@renderer/api/api'
import { OpenAPI } from '@renderer/api'

type Props = { sessionId: string | null }

type Session = { id: string; title: string; createdAt: number; updatedAt: number }

const ChatMessages = ({ sessionId }: Props) => {
  const theme = useTheme()
  const [msgs, setMsgs] = useState<MessageDto[]>([])

  // const token = useMemo(() => localStorage.getItem('token') ?? '', [])
  const base =
    OpenAPI.BASE || (import.meta.env.VITE_API_BASE_URL as string) || 'http://localhost:5164'

  const getToken = () => localStorage.getItem('token') ?? ''

  // Load existing messages whenever the selected session changes
  useEffect(() => {
    try {
      const raw = localStorage.getItem(`chat.messages:${sessionId}`) || '[]'
      setMsgs(JSON.parse(raw))
    } catch {
      setMsgs([])
    }
  }, [sessionId])

  // autoscroll
  const endRef = useRef<HTMLDivElement | null>(null)
  useEffect(() => {
    endRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [msgs])

  // useEffect(() => {
  //   if (!getToken()) return

  //   const conn = new HubConnectionBuilder()
  //     .withUrl(`${base}/chat`, { accessTokenFactory: getToken })
  //     .configureLogging(LogLevel.Information)
  //     .withAutomaticReconnect()
  //     .build()

  //   const handler = (sender: string, text: string) => {
  //     const dto: MessageDto = {
  //       id: Date.now(),
  //       userId: 0,
  //       content: text,
  //       sentAt: new Date().toISOString(),
  //       username: sender as any
  //     } as any

  //     setMsgs((prev) => {
  //       const next = [...prev, dto]
  //       // persist messages for this session
  //       localStorage.setItem(`chat.messages:${sessionId}`, JSON.stringify(next))

  //       // update sessions list:
  //       try {
  //         const sessions: Session[] = JSON.parse(localStorage.getItem('chat.sessions') || '[]')
  //         const idx = sessions.findIndex((s) => s.id === sessionId)
  //         if (idx >= 0) {
  //           const s = { ...sessions[idx] }

  //           // Title from FIRST **user** message only
  //           const isUser = sender.toLowerCase() !== 'ai'
  //           if ((s.title === 'New chat' || !s.title) && isUser) {
  //             s.title = text.slice(0, 40)
  //           }

  //           // Keep createdAt fixed; only bump updatedAt for sorting
  //           s.updatedAt = Date.now()

  //           const updated = [...sessions]
  //           updated[idx] = s
  //           // (optional) sort by recent activity
  //           updated.sort((a, b) => b.updatedAt - a.updatedAt)

  //           localStorage.setItem('chat.sessions', JSON.stringify(updated))
  //         }
  //       } catch {
  //         /* ignore */
  //       }

  //       return next
  //     })
  //   }

  //   conn.on('ReceiveMessage', handler)
  //   conn.start().catch((err) => console.error('[SignalR] start error', err))
  //   return () => {
  //     conn.off('ReceiveMessage', handler)
  //     void conn.stop()
  //   }
  // }, [base, sessionId])

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

      setMsgs((prev) => {
        const next = [...prev, dto]
        localStorage.setItem(`chat.messages:${sessionId}`, JSON.stringify(next))
        return next
      })
    }

    conn.on('ReceiveMessage', handler)

    let cancelled = false
    const start = async () => {
      try {
        await conn.start()
      } catch (err) {
        // retry a bit later if start races during reload
        if (!cancelled) setTimeout(start, 1500)
      }
    }
    start()

    // when SignalR reconnected, reload the persisted messages for this session
    conn.onreconnected(() => {
      try {
        const raw = localStorage.getItem(`chat.messages:${sessionId}`) || '[]'
        setMsgs(JSON.parse(raw))
      } catch {
        setMsgs([])
      }
    })

    return () => {
      cancelled = true
      conn.off('ReceiveMessage', handler)
      void conn.stop()
    }
  }, [base, sessionId]) // <-- note: no captured `token` here

  return (
    <Box sx={styles.ChatMessages(theme)}>
      {msgs.map((m, i) => (
        <Box key={i} gap={1} sx={{ mb: 2, display: 'flex' }}>
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
