import { resolve } from 'path'
import { defineConfig, externalizeDepsPlugin } from 'electron-vite'
import react from '@vitejs/plugin-react'
import dotenv from 'dotenv'
import path from 'path'
import history from 'connect-history-api-fallback'

dotenv.config({ path: path.resolve(__dirname, '../.env') })

const spaFallbackPlugin = () => {
  return {
    name: 'spa-fallback',
    configureServer(server) {
      server.middlewares.use(history())
    }
  }
}

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
    plugins: [react(), spaFallbackPlugin()],
    server: {
      host: '0.0.0.0',
      port: 5173,
      strictPort: true
    }
  }
})
