import { useState } from 'react'
import { useNavigate, Link as RouterLink } from 'react-router-dom'
import { Box, Button, Link, Stack, TextField, Typography } from '@mui/material'
import { api } from '@renderer/api/api'
import { OpenAPI } from '@renderer/api'

export default function RegisterPage() {
  const [username, setU] = useState('')
  const [password, setP] = useState('')
  const [err, setErr] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)
  const nav = useNavigate()

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErr(null)
    setBusy(true)
    try {
      await api.AuthService.postApiAuthRegister({ username, password }) // 201 Created
      const token = await api.AuthService.postApiAuthLogin({ username, password })
      localStorage.setItem('token', token)
      OpenAPI.TOKEN = token
      nav('/chat', { replace: true })
    } catch (e: any) {
      setErr(e?.message ?? 'Registration failed')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Box component="form" onSubmit={onSubmit} sx={{ maxWidth: 360, m: '4rem auto' }}>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Create account
      </Typography>
      <Stack spacing={2}>
        <TextField
          label="Username"
          value={username}
          onChange={(e) => setU(e.target.value)}
          autoFocus
        />
        <TextField
          label="Password"
          type="password"
          value={password}
          onChange={(e) => setP(e.target.value)}
        />
        <Button
          type="submit"
          variant="contained"
          disabled={busy || !username.trim() || !password.trim()}
        >
          {busy ? '...' : 'Register'}
        </Button>
        {!!err && <Typography color="error">{err}</Typography>}
        <Typography variant="body2" sx={{ mt: 1 }}>
          Already have an account?{' '}
          <Link component={RouterLink} to="/login">
            Sign in
          </Link>
        </Typography>
      </Stack>
    </Box>
  )
}
