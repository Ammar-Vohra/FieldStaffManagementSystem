
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react";

import { useAuth } from "./AuthContext";
import userPermissionService from "../services/userPermissionService";

const PermissionContext = createContext(null);

export const PermissionProvider = ({ children }) => {
  const { token, isAuthenticated } = useAuth();

  const [permissions, setPermissions] = useState([]);
  const [loadingPermissions, setLoadingPermissions] = useState(false);

  const fetchPermissions = useCallback(async () => {
    if (!token) {
      setPermissions([]);
      return;
    }

    try {
      setLoadingPermissions(true);

      const result = await userPermissionService.getMyPermissions();

      setPermissions(Array.isArray(result) ? result : []);
    } catch (error) {
      console.error("Failed to fetch permissions:", error);
      setPermissions([]);
    } finally {
      setLoadingPermissions(false);
    }
  }, [token]);

  useEffect(() => {
    if (isAuthenticated) {
      fetchPermissions();
    } else {
      setPermissions([]);
    }
  }, [isAuthenticated, fetchPermissions]);

  const hasPermission = useCallback(
    (permissionCode) => {
      return permissions.includes(permissionCode);
    },
    [permissions]
  );

  const value = {
    permissions,
    loadingPermissions,
    hasPermission,
    refreshPermissions: fetchPermissions,
  };

  return (
    <PermissionContext.Provider value={value}>
      {children}
    </PermissionContext.Provider>
  );
};

export const usePermissions = () => {
  const context = useContext(PermissionContext);

  if (!context) {
    throw new Error(
      "usePermissions must be used inside PermissionProvider"
    );
  }

  return context;
};
