import { useEffect, useState, useCallback } from 'react'
import { api, MessageDto } from '@renderer/api/api'

// This function now maps directly to your new C# endpoint
const fetchMessages = async (sessionId: number): Promise<MessageDto[]> => {
  // The 'getApiChatMessages' name comes from the [SwaggerOperation(OperationId = "...")]
  // Your generated client might take an object:
  // const resp = await api.ChatService.getApiChatMessages({ conversationId: sessionId });
  // Or just the ID:
  const resp = await api.ChatService.getConversationMessages(sessionId)
  return resp || []
}

export function useChatMessages(sessionId: number | null) {
  const [messages, setMessages] = useState<MessageDto[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<Error | null>(null)

  const loadMessages = useCallback(async () => {
    if (sessionId == null) {
      setMessages([])
      setLoading(false)
      setError(null)
      return
    }

    setLoading(true)
    setError(null)
    try {
      const data = await fetchMessages(0)
      setMessages(data)
    } catch (err: any) {
      console.error('Failed to fetch messages:', err)
      setError(err)
      setMessages([])
    } finally {
      setLoading(false)
    }
  }, [sessionId])

  useEffect(() => {
    loadMessages()
  }, [loadMessages]) // depends on sessionId via useCallback

  // Return messages, the setter, loading/error, and the refetch function
  return { messages, setMessages, loading, error, refetchMessages: loadMessages }
}
