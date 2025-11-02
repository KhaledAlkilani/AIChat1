export interface AuthUser {
  id: number
  username: string
  roles: string[]
}

export const getTokenString = (): string | null => localStorage.getItem('token')

export function getUserFromToken(token: string): AuthUser | null {
  try {
    const payloadBase64 = token.split('.')[1]
    const json = atob(payloadBase64.replace(/-/g, '+').replace(/_/g, '/'))
    const payload = JSON.parse(json) as Record<string, any>

    // We issued these in JwtIssuer: sub (userId), ClaimTypes.Name -> ends up as "name" or the long URI
    const id = Number(payload.sub)
    const username =
      payload.name ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']

    // role may be "role" or URI, string or array
    let roles =
      payload.role ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    roles = Array.isArray(roles) ? roles : roles ? [roles] : []

    if (!id || !username) return null
    return { id, username, roles }
  } catch {
    return null
  }
}

export function logout() {
  localStorage.removeItem('token')
  localStorage.removeItem('chat.active')

  // Optionally clear all cached chat data
  Object.keys(localStorage).forEach((key) => {
    if (key.startsWith('chat.messages:')) {
      localStorage.removeItem(key)
    }
  })
}
