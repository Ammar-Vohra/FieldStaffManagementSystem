
import { createContext, useContext, useState } from "react";
import authService from "../services/authservice";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const storedUser = localStorage.getItem("user");
    return storedUser ? JSON.parse(storedUser) : null;
  });

  const [token, setToken] = useState(() => {
    return localStorage.getItem("token");
  });

  // Login
  const login = async (credentials) => {
    const response = await authService.login(credentials);

    if (!response.token) {
      throw new Error("JWT token was not returned by the server.");
    }

    const userData = {
      userId: response.userId,
      userName: response.userName,
    //   email: response.email,
      roleId: response.roleId,
    };

    localStorage.setItem("token", response.token);
    localStorage.setItem("user", JSON.stringify(userData));

    setToken(response.token);
    setUser(userData);

    return response;
  };

  // Logout
  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");

    setToken(null);
    setUser(null);
  };

  const value = {
    user,
    token,
    isAuthenticated: !!token,
    login,
    logout,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook
export const useAuth = () => {
  return useContext(AuthContext);
};
