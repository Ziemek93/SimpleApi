import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './app/layout/index.css'
import {RouterProvider} from "react-router";
import {router} from "./app/Routes.tsx";
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <QueryClientProvider client={queryClient}>
      
    <RouterProvider router={router} />
      </QueryClientProvider>
          
  </StrictMode>,
)
