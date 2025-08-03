import Box from '@mui/material/Box'
import SideBar from './components/SideBar'
import ChatWindow from './components/ChatWindow'

const App = () => {
  return (
    <Box sx={styles.appContainer()}>
      <SideBar />
      <ChatWindow />
    </Box>
  )
}

const styles = {
  appContainer: () => ({
    display: 'flex',
    flexDirection: 'row',
    height: '100vh'
  })
}

export default App
