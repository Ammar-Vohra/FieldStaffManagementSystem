import { Navigate } from "react-router-dom";
import { usePermissions } from "../../context/PermissionContext";

const PermissionGuard = ({ permission, children }) => {
  const { hasPermission, loadingPermissions } = usePermissions();

  if (loadingPermissions) {
    return (
      <div className="flex min-h-[60vh] items-center justify-center">
        <div className="flex flex-col items-center gap-3">
          <div className="h-10 w-10 animate-spin rounded-full border-4 border-gray-200 border-t-blue-600" />
          <p className="text-sm text-gray-500">
            Checking permissions...
          </p>
        </div>
      </div>
    );
  }

  if (!hasPermission(permission)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
};

export default PermissionGuard;