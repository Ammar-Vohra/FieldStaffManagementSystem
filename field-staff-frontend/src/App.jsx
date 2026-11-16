
import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";

import { AuthProvider } from "./context/AuthContext";

import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";

import ProtectedRoute from "./components/common/ProtectedRoute";
import MainLayout from "./components/layout/MainLayout";
import Roles from "./pages/Role";
import Users from "./pages/User";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>

       
          <Route path="/login" element={<Login />} />

      
          <Route
            element={
              <ProtectedRoute>
                <MainLayout />
              </ProtectedRoute>
            }
          >
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/roles" element={<Roles />} />
            <Route path="/users" element={<Users />} />



            
            <Route
              path="*"
              element={
                <h2 className="text-xl font-semibold">
                  Page coming soon...
                </h2>
              }
            />
          </Route>

       
          <Route
            path="/"
            element={<Navigate to="/login" replace />}
          />

        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
