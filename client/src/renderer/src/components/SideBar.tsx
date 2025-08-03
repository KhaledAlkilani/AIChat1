import { Box, Typography } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'

const SideBar = () => {
  const theme = useTheme()

  return (
    <Box sx={styles.sidebar(theme)}>
      <Typography color="black" variant="subtitle1">
        Chat List
      </Typography>
    </Box>
  )
}

export default SideBar

const styles = {
  sidebar: (theme: Theme) => ({
    width: '24%',
    bgcolor: theme.palette.grey[200],
    p: 2,
    display: 'flex',
    flexDirection: 'column'
  })
}
