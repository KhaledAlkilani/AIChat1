import './assets/base.css'

import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App'
import { OpenAPI } from './api'

OpenAPI.BASE = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:5164'
console.log('[OpenAPI.BASE]', OpenAPI.BASE)

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>
)
