import { OpenAPI } from '@renderer/api'

type AuthState = {
  token: string | null
  setToken: (t: string | null) => void
}

const auth: AuthState = {
  token: null,
  setToken(t) {
    auth.token = t
    // feed token to generated client
    OpenAPI.TOKEN = t ? `Bearer ${t}` : undefined
    // dev-only persistence
    if (t) localStorage.setItem('jwt', t)
    else localStorage.removeItem('jwt')
  }
}

// rehydrate on refresh (dev)
const saved = localStorage.getItem('jwt')
if (saved) auth.setToken(saved)

export default auth
