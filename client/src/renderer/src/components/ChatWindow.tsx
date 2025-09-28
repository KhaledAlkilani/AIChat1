// import Header from './Header'
// import ChatMessages from './ChatMessages'
// import Footer from './Footer'
// import { Box } from '@mui/material'

// const ChatWindow = () => {
//   return (
//     <Box sx={styles.chatWindow}>
//       <Header />
//       <ChatMessages />
//       <Footer />
//     </Box>
//   )
// }

// export default ChatWindow

// const styles = {
//   chatWindow: {
//     display: 'flex',
//     flexDirection: 'column',
//     flexGrow: 1,
//     height: '100%'
//   }
// }

import Header from './Header'
import ChatMessages from './ChatMessages'
import Footer from './Footer'
import SideBar from './SideBar'
import { Box, Typography } from '@mui/material'
import { useEffect, useState } from 'react'
import { getTokenString, getUserFromToken } from '@renderer/auth/jwt'
import { OpenAPI } from '@renderer/api'
import { api } from '@renderer/api/api'

type Session = { id: string; title: string; createdAt: number; updatedAt: number }

function createSession(): Session {
  return {
    id: crypto.randomUUID(),
    title: 'New chat',
    createdAt: Date.now(),
    updatedAt: Date.now()
  }
}
const ChatWindow = () => {
  const [activeSessionId, setActiveSessionId] = useState<string | null>(null)

  useEffect(() => {
    const last = localStorage.getItem('chat.active')
    console.log('[last active chat id]', last)
  }, [])

  const ensureSessions = () => {
    const raw = localStorage.getItem('chat.sessions')
    if (!raw) localStorage.setItem('chat.sessions', '[]')
  }
  ensureSessions()

  const handleCreateNewConversation = async () => {
    // 0) ensure we have the current user id
    const t = getTokenString()
    const me = t ? getUserFromToken(t) : null
    if (!me) {
      console.warn('No user — cannot start server conversation')
    } else {
      try {
        await api.ChatService.startNewConversation({
          userId: me.id
        })
        // we don't need the returned id here; Send() will pick the new active conv
      } catch (e) {
        console.error('Failed to start server conversation', e)
      }
    }

    // 1) keep your local session for the UI
    const s = createSession()
    const sessions: Session[] = JSON.parse(localStorage.getItem('chat.sessions') || '[]')
    localStorage.setItem('chat.sessions', JSON.stringify([s, ...sessions]))
    localStorage.setItem(`chat.messages:${s.id}`, '[]')

    // 2) select & remember it (UI only)
    setActiveSessionId(s.id)
    localStorage.setItem('chat.active', s.id)
  }

  // when selecting from sidebar
  const handleSelectExistingConversation = (id: string | null) => {
    setActiveSessionId(id)
    if (id) localStorage.setItem('chat.active', id)
    else localStorage.removeItem('chat.active') // cleared after delete
  }

  return (
    <Box sx={styles.appContainer()}>
      {/* this row holds SideBar + chat column side-by-side */}
      <Box sx={styles.mainRow}>
        <SideBar
          active={activeSessionId}
          onSelect={handleSelectExistingConversation}
          onCreate={handleCreateNewConversation}
        />
        {/* chat column */}
        <Box sx={styles.chatColumn}>
          <Header />
          {activeSessionId ? (
            <>
              <ChatMessages sessionId={activeSessionId} />
              <Footer sessionId={activeSessionId} />
            </>
          ) : (
            // Empty state when nothing selected
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
    display: 'flex',
    flexDirection: 'row',
    height: '100vh',
    width: '100vw'
  }),
  mainRow: {
    display: 'flex',
    flexDirection: 'row',
    flexGrow: 1,
    width: '100%',
    minHeight: 0
  },
  chatColumn: {
    display: 'flex',
    flexDirection: 'column',
    flexGrow: 1,
    minWidth: 0,
    height: '100%'
  }
}
