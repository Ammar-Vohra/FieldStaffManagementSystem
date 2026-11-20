
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
import UserPermissions from "./pages/UserPermission";
import Menu from "./pages/Menu";
import ServiceTasks from "./pages/ServiceTask";
import Customers from "./pages/Customer";
import TaskPriority from "./pages/TaskPriority";
import TaskStatus from "./pages/TaskStatus";
import TaskReport from "./pages/TaskReport";
import StaffReport from "./pages/StaffReport";
import MyTasks from "./pages/MyTask";

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
            <Route path="/user-permissions" element={<UserPermissions />} />
            <Route path="/menus" element={<Menu />} />
            <Route path="/tasks" element={<ServiceTasks />} />
            <Route path="/customers" element={<Customers />} />
            <Route path="/customers" element={<Customers />} />
            <Route path="/task-priorities" element={<TaskPriority />} />
            <Route path="/task-statuses" element={<TaskStatus />} />
            <Route path="/reports/tasks" element={<TaskReport />} />
            <Route path="/reports/staff" element={<StaffReport />} />
            <Route path="/my-tasks" element={<MyTasks />} />

            







            
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
