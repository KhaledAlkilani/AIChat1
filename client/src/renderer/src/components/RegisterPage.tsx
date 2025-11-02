import { useState } from 'react'
import { useNavigate, Link as RouterLink } from 'react-router-dom'
import { Box, Button, Link, Stack, TextField, Typography } from '@mui/material'
import { api, LoginRequest } from '@renderer/api/api'
import { OpenAPI } from '@renderer/api'

export default function RegisterPage() {
  const [userData, setUserData] = useState<LoginRequest>({ username: '', password: '' })
  const [err, setErr] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)
  const nav = useNavigate()

  const handleUsernameChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setUserData({
      ...userData,
      username: e.target.value
    })
  }

  const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setUserData({
      ...userData,
      password: e.target.value || ''
    })
  }

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setErr(null)
    setBusy(true)
    try {
      await api.AuthService.postApiAuthRegister(userData) // 201 Created
      const token = await api.AuthService.postApiAuthLogin(userData)
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
          value={userData?.username}
          onChange={handleUsernameChange}
          autoFocus
        />
        <TextField
          label="Password"
          type="password"
          value={userData?.password}
          onChange={handlePasswordChange}
        />
        <Button
          type="submit"
          variant="contained"
          disabled={busy || !userData?.username.trim() || !userData.password.trim()}
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
