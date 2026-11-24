import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";

import { AuthProvider } from "./context/AuthContext";
import { PermissionProvider } from "./context/PermissionContext";

import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
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
import NotFound from "./pages/NotFound";
import Unauthorized from "./pages/Unauthorized";

import ProtectedRoute from "./components/common/ProtectedRoute";
import PermissionGuard from "./components/auth/PermissionGuard";
import MainLayout from "./components/layout/MainLayout";

function App() {
  return (
    <AuthProvider>
      <PermissionProvider>
        <BrowserRouter>
          <Routes>

            {/* Public Routes */}
            <Route path="/login" element={<Login />} />

            {/* Authenticated Routes */}
            <Route
              element={
                <ProtectedRoute>
                  <MainLayout />
                </ProtectedRoute>
              }
            >
              {/* Dashboard */}
              <Route
                path="/dashboard"
                element={
                  <PermissionGuard permission="DASHBOARD_VIEW">
                    <Dashboard />
                  </PermissionGuard>
                }
              />

              {/* User Management */}
              <Route
                path="/users"
                element={
                  <PermissionGuard permission="USER_VIEW">
                    <Users />
                  </PermissionGuard>
                }
              />

              <Route
                path="/roles"
                element={
                  <PermissionGuard permission="ROLE_VIEW">
                    <Roles />
                  </PermissionGuard>
                }
              />

              <Route
                path="/user-permissions"
                element={
                  <PermissionGuard permission="USER_PERMISSION_MANAGE">
                    <UserPermissions />
                  </PermissionGuard>
                }
              />

              {/* Menu Management */}
              <Route
                path="/menus"
                element={
                  <PermissionGuard permission="MENU_VIEW">
                    <Menu />
                  </PermissionGuard>
                }
              />

              {/* Task Management */}
              <Route
                path="/tasks"
                element={
                  <PermissionGuard permission="TASK_VIEW_ALL">
                    <ServiceTasks />
                  </PermissionGuard>
                }
              />

              <Route
                path="/my-tasks"
                element={
                  <PermissionGuard permission="TASK_VIEW_ASSIGNED">
                    <MyTasks />
                  </PermissionGuard>
                }
              />

              <Route
                path="/task-priorities"
                element={
                  <PermissionGuard permission="TASK_VIEW_ALL">
                    <TaskPriority />
                  </PermissionGuard>
                }
              />

              <Route
                path="/task-statuses"
                element={
                  <PermissionGuard permission="TASK_VIEW_ALL">
                    <TaskStatus />
                  </PermissionGuard>
                }
              />

              {/* Customer Management */}
              <Route
                path="/customers"
                element={
                  <PermissionGuard permission="CUSTOMER_VIEW">
                    <Customers />
                  </PermissionGuard>
                }
              />

              {/* Reports */}
              <Route
                path="/reports/tasks"
                element={
                  <PermissionGuard permission="TASK_VIEW_ALL">
                    <TaskReport />
                  </PermissionGuard>
                }
              />

              <Route
                path="/reports/staff"
                element={
                  <PermissionGuard permission="USER_VIEW">
                    <StaffReport />
                  </PermissionGuard>
                }
              />

            </Route>

            {/* Unauthorized */}
            <Route path="/unauthorized" element={<Unauthorized />} />

            {/* Root */}
            <Route
              path="/"
              element={<Navigate to="/login" replace />}
            />

            {/* Not Found */}
            <Route path="*" element={<NotFound />} />

          </Routes>
        </BrowserRouter>
      </PermissionProvider>
    </AuthProvider>
  );
}

export default App;