import { createBrowserRouter, Navigate } from "react-router";
import App from "./layout/App.tsx";
import NotFound from "./features/errors/NotFound.tsx";

export const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        children: [
            
        ],
    },
    { path: 'not-found', element: <NotFound /> },
    
    {path: '*', element: <Navigate replace to='/not-found' /> }
])