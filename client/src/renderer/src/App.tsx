import ChatWindow from './components/ChatWindow'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { JSX } from 'react'
import LoginPage from './components/LoginPage'
import RegisterPage from './components/RegisterPage'
import { OpenAPI } from './api'

const RequireAuth = ({ children }: { children: JSX.Element }) => {
  const token = localStorage.getItem('token')
  if (!token) return <Navigate to="/login" replace />
  if (!OpenAPI.TOKEN) OpenAPI.TOKEN = token
  return children
}

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route
          path="/chat"
          element={
            <RequireAuth>
              <ChatWindow />
            </RequireAuth>
          }
        />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
