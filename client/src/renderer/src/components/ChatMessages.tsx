import { Box } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'

const ChatArea = () => {
  const theme = useTheme()

  return <Box sx={styles.chatArea(theme)}>{'Chat messages'}</Box>
}

export default ChatArea

const styles = {
  chatArea: (theme: Theme) => ({
    flexGrow: 1,
    p: 2,
    overflowY: 'auto',
    bgcolor: theme.palette.background.paper
  })
}
