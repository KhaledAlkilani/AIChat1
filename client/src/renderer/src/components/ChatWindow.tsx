import Header from './Header'
import ChatMessages from './ChatMessages'
import Footer from './Footer'
import { Box } from '@mui/material'

const ChatWindow = () => {
  return (
    <Box sx={styles.chatWindow}>
      <Header />
      <ChatMessages />
      <Footer />
    </Box>
  )
}

export default ChatWindow

const styles = {
  chatWindow: {
    display: 'flex',
    flexDirection: 'column',
    flexGrow: 1,
    height: '100%'
  }
}
