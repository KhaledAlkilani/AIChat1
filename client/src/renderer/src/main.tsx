import './assets/base.css'
import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App'
import { OpenAPI } from './api'

OpenAPI.BASE = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5164'
const bootToken = localStorage.getItem('token')
if (bootToken) OpenAPI.TOKEN = bootToken

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>
)
