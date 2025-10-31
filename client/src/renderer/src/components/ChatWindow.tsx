import { useChatSessions } from '../hooks/useChatSessions'
import { getTokenString, getUserFromToken } from '@renderer/auth/jwt'
import { api } from '@renderer/api/api'
import { Box, Typography } from '@mui/material'
import ChatMessages from './ChatMessages'
import SideBar from './SideBar'
import Header from './Header'
import { useEffect, useState } from 'react'
import Footer from './Footer'

const ChatWindow = () => {
  const { sessions, userId, setSessions, refetchSessions } = useChatSessions(true)
  const [activeSessionId, setActiveSessionId] = useState<number | null>(null)
  const [isCreatingNew, setIsCreatingNew] = useState(false)

  useEffect(() => {
    if (!sessions.length || activeSessionId !== null) return
    const sorted = [...sessions].sort(
      (a, b) => new Date(b.createdAt || 0).getTime() - new Date(a.createdAt || 0).getTime()
    )
    if (sorted[0]?.id) setActiveSessionId(sorted[0].id)
  }, [sessions, activeSessionId])

  const handleCreateNewConversation = async () => {
    if (!userId) {
      return
    }

    const me = getUserFromToken(getTokenString()!)
    if (!me) {
      console.warn('Invalid token / user')
      return
    }

    try {
      // Backend call to start a new conversation
      const created = await api.ChatService.startNewConversation({ userId: me.id }) // now returns ConversationDto
      if (!created || typeof created.id !== 'number') return
      setSessions((prev) => [created, ...prev])
      setActiveSessionId(created.id)
      void refetchSessions()
    } catch (e) {
      console.error('Backend startNewConversation failed', e)
    }
  }

  const handleSelectExisting = (id: number | null) => {
    setActiveSessionId(id)
    setIsCreatingNew(false) // We are now viewing an existing session
  }

  return (
    <Box sx={styles.appContainer()}>
      <Box sx={styles.mainRow}>
        <SideBar
          active={activeSessionId}
          onSelect={handleSelectExisting}
          onCreate={handleCreateNewConversation}
          sessions={sessions}
          userId={userId}
          setSessions={setSessions}
        />
        <Box sx={styles.chatColumn}>
          <Header />
          {activeSessionId || isCreatingNew ? (
            // Show chat UI if we have an ID OR if we are creating a new one
            <>
              <ChatMessages sessionId={activeSessionId} />
              <Footer
                sessionId={activeSessionId!}
                onMessageSent={async (firstUserText?: string, sessionIdArg?: number) => {
                  // optimistic title from the first user message if missing
                  const sid = sessionIdArg ?? activeSessionId
                  if (firstUserText && activeSessionId) {
                    setSessions((prev) =>
                      prev.map((s) =>
                        s.id === sid && (!s.title || s.title === 'New chat')
                          ? { ...s, title: firstUserText.slice(0, 60) }
                          : s
                      )
                    )
                  }
                  await refetchSessions()
                  setIsCreatingNew(false)
                }}
              />
            </>
          ) : (
            <Box sx={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <Typography color="text.secondary">
                Pick a conversation or create a new one
              </Typography>
            </Box>
          )}
        </Box>
      </Box>
    </Box>
  )
}

export default ChatWindow

const styles = {
  appContainer: () => ({
    height: '100vh',
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden'
  }),
  mainRow: {
    display: 'flex',
    flex: 1,
    overflow: 'hidden'
  },
  chatColumn: {
    flex: 1,
    display: 'flex',
    flexDirection: 'column',
    overflow: 'hidden'
  }
}
