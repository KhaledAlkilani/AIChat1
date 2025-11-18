import { useState } from 'react'
import { OpenAPI } from '@renderer/api'
import { Box, Button, Link, Stack, TextField, Typography } from '@mui/material'
import { api } from '@renderer/api/api'
import { useNavigate, Link as RouterLink } from 'react-router-dom'

const styles = {
  formContainer: {
    maxWidth: 360,
    margin: '4rem auto'
  },
  title: {
    marginBottom: 2
  },
  errorText: {
    color: 'error.main'
  },
  registerText: {
    marginTop: 1
  }
}

const LoginPage = () => {
  const navigate = useNavigate()

  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)
    setIsLoading(true)

    try {
      const token = await api.AuthService.postApiAuthLogin({ username, password })
      localStorage.setItem('token', token)
      OpenAPI.TOKEN = token
      navigate('/chat', { replace: true })
    } catch (err: any) {
      setError(err?.message ?? 'Login failed')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <Box component="form" onSubmit={handleLogin} sx={styles.formContainer}>
      <Typography variant="h5" sx={styles.title}>
        Sign in
      </Typography>

      <Stack spacing={2}>
        <TextField
          id="username_field"
          label="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          autoFocus
        />
        <TextField
          id="password_field"
          label="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <Button
          type="submit"
          variant="contained"
          disabled={isLoading || !username.trim() || !password.trim()}
          id="submit_button"
        >
          {isLoading ? '...' : 'Login'}
        </Button>

        {error && <Typography sx={styles.errorText}>{error}</Typography>}

        <Typography variant="body2" sx={styles.registerText}>
          Don’t have an account?{' '}
          <Link id="register-click" component={RouterLink} to="/register">
            Register
          </Link>
        </Typography>
      </Stack>
    </Box>
  )
}

export default LoginPage
