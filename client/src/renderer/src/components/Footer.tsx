import { Box, FilledInput, Input, InputBase } from '@mui/material'
import { useTheme, Theme } from '@mui/material/styles'
import FormControl from '@mui/material/FormControl'
import OutlinedInput from '@mui/material/OutlinedInput'

const Footer = () => {
  const theme = useTheme()
  return (
    <Box sx={styles.footer(theme)}>
      <FormControl sx={{ width: '100%' }}>
        <InputBase
          sx={{ padding: theme.spacing(0.5) }}
          fullWidth
          placeholder="Ask what on your mind"
        />
      </FormControl>
    </Box>
  )
}

export default Footer

const styles = {
  footer: (theme: Theme) => ({
    gridColumn: '2 / 3',
    paddingTop: theme.spacing(2),
    paddingBottom: theme.spacing(2),
    paddingRight: theme.spacing(1),
    paddingLeft: theme.spacing(1),
    backgroundColor: theme.palette.grey[100],
    borderTop: `1px solid ${theme.palette.divider}`,
    color: theme.palette.text.secondary
  })
}
