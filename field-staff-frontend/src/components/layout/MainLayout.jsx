
import { useEffect, useState } from "react";
import { Outlet, useNavigate } from "react-router-dom";

import { useAuth } from "../../context/AuthContext";
import roleMenuService from "../../services/roleMenuService";
import Sidebar from "./SideBar";

function MainLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [menus, setMenus] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchMenus = async () => {
      try {
        setLoading(true);
        setError("");

        const data = await roleMenuService.getMyMenu();

        setMenus(data.filter((menu) => menu.isActive));
      } catch (err) {
        console.error("Failed to fetch menus:", err);

        setError(
          err.response?.data?.message ||
          err.response?.data?.title ||
          "Unable to load your menus."
        );
      } finally {
        setLoading(false);
      }
    };

    fetchMenus();
  }, []);

  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  const roleName = menus[0]?.roleName || "User";

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        Loading your dashboard...
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center gap-4">
        <p className="text-red-600">{error}</p>

        <button
          onClick={() => window.location.reload()}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen bg-slate-100">

      <Sidebar
        menus={menus}
        roleName={roleName}
        userName={user?.userName}
        onLogout={handleLogout}
      />

      <main className="flex-1 p-6">
        <Outlet context={{ roleName }} />
      </main>

    </div>
  );
}

export default MainLayout;
