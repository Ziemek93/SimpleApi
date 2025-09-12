import { Box, CssBaseline } from "@mui/material";
import './styles.css'
import TopNavbar from "./Navbar.tsx";
import PostList from "../features/posts/PostList.tsx";

function App() {

  return (
      <Box sx={{ display: 'flex', flexDirection: 'column', bgcolor: '#eeeeee', minHeight: '100vh' }}>
          <CssBaseline />
          
        <TopNavbar/>
          <Box
              component="main"
              sx={{
                  flexGrow: 1,
                  display: 'flex',
                  flexDirection: 'column',
                  justifyContent: 'center',
                  alignItems: 'center',
                  gap: 2
              }}
          >
              <h1>Vite + React</h1>
              <PostList />
          </Box>
      </Box>
  )
}

export default App
