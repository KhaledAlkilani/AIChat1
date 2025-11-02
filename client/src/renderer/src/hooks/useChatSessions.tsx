import { useEffect, useState, useCallback } from 'react'
import { getTokenString, getUserFromToken } from '@renderer/auth/jwt'
import { api, ConversationDto } from '@renderer/api/api'

export function useChatSessions(connect: boolean = true) {
  const [sessions, setSessions] = useState<ConversationDto[]>([])
  const [userId, setUserId] = useState<number | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<Error | null>(null)

  // Determine user on mount
  useEffect(() => {
    const token = getTokenString()
    const user = token ? getUserFromToken(token) : null
    setUserId(user?.id ?? null)
  }, [])

  const fetchSessions = useCallback(async () => {
    if (userId == null) {
      setSessions([])
      setLoading(false)
      return
    }

    setLoading(true)
    setError(null)
    try {
      const resp = await api.ChatService.getConversations(userId)
      setSessions(resp || [])
    } catch (err: any) {
      console.error('Failed to fetch conversations:', err)
      setError(err)
      setSessions([])
    } finally {
      setLoading(false)
    }
  }, [userId])

  // Fetch backend sessions
  useEffect(() => {
    if (connect && userId != null) {
      fetchSessions()
    } else {
      // If not connecting or no user, clear data
      setSessions([])
      setLoading(false)
      setError(null)
    }
  }, [userId, connect, fetchSessions])

  // Expose setSessions for optimistic updates and refetch for explicit updates
  return { loading, error, sessions, userId, setSessions, refetchSessions: fetchSessions }
}
