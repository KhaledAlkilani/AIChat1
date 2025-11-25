import { resolve } from 'path'
import { defineConfig, externalizeDepsPlugin } from 'electron-vite'
import react from '@vitejs/plugin-react'
import dotenv from 'dotenv'
import path from 'path'

dotenv.config({ path: path.resolve(__dirname, '../.env') })

export default defineConfig({
  main: {
    plugins: [externalizeDepsPlugin()]
  },
  preload: {
    plugins: [externalizeDepsPlugin()]
  },
  renderer: {
    resolve: {
      alias: {
        '@renderer': resolve('src/renderer/src'),
        '@mui/styled-engine': '@mui/styled-engine-sc'
      }
    },
    plugins: [react()],
    server: {
      host: '0.0.0.0', // so Jenkins / other containers can reach it
      port: 5173,
      strictPort: true,
      allowedHosts: ['host.docker.internal']
    }
  }
})
