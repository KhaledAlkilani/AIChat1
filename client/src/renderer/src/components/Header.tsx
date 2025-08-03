import { Box, Typography } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'

const Header = () => {
  const theme = useTheme()

  return (
    <Box sx={styles.header(theme)}>
      <Typography color="black" variant="h6">
        AI Chat App
      </Typography>
    </Box>
  )
}

export default Header

const styles = {
  header: (theme: Theme) => ({
    gridColumn: '2 / 3',
    padding: theme.spacing(1),
    backgroundColor: theme.palette.grey[100],
    borderBottom: `1px solid ${theme.palette.divider}`
  })
}
