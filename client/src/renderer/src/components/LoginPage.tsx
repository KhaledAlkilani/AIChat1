import { useState } from 'react'
import { OpenAPI } from '@renderer/api'
import { Box, Button, Link, Stack, TextField, Typography } from '@mui/material'
import { api } from '@renderer/api/api'
import { useNavigate, Link as RouterLink } from 'react-router-dom'

const LoginPage = () => {
  const nav = useNavigate()

  const [username, setU] = useState('')
  const [password, setP] = useState('')
  const [err, setErr] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErr(null)
    setBusy(true)
    try {
      const token = await api.AuthService.postApiAuthLogin({ username, password }) // returns a string
      localStorage.setItem('token', token)
      OpenAPI.TOKEN = token
      nav('/chat', { replace: true }) // <— use router navigation
    } catch (e: any) {
      setErr(e?.message ?? 'Login failed')
    } finally {
      setBusy(false)
    }
  }

  return (
    <Box component="form" onSubmit={onSubmit} sx={{ maxWidth: 360, m: '4rem auto' }}>
      <Typography variant="h5" sx={{ mb: 2 }}>
        Sign in
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
          {busy ? '...' : 'Login'}
        </Button>
        {!!err && <Typography color="error">{err}</Typography>}
        <Typography variant="body2" sx={{ mt: 1 }}>
          Don’t have an account?{' '}
          <Link component={RouterLink} to="/register">
            Register
          </Link>
        </Typography>
      </Stack>
    </Box>
  )
}

export default LoginPage
