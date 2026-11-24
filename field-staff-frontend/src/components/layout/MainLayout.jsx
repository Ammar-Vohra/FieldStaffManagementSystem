
import { useEffect, useState } from "react";
import { Outlet, useNavigate } from "react-router-dom";

import { useAuth } from "../../context/AuthContext";
import roleMenuService from "../../services/roleMenuService";
// import { buildMenuTree } from "../../utils/menuUtils";
import Sidebar from "./SideBar";

function MainLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [menus, setMenus] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const fetchMenus = async () => {
    try {
      setLoading(true);
      setError("");

      const data = await roleMenuService.getMyMenu();

      const activeMenus = data.filter(
        (menu) => menu.isActive
      );

      setMenus(activeMenus);
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

  useEffect(() => {
    fetchMenus();
  }, []);

  const handleLogout = () => {
    logout();
    navigate("/login", { replace: true });
  };

  const roleName = menus[0]?.roleName || "User";

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-100">
        <p className="text-slate-600">Loading your dashboard...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center gap-4 bg-slate-100 p-4">
        <p className="text-red-600 text-center">{error}</p>

        <button
          onClick={fetchMenus}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-100">
      <Sidebar
        menus={menus}
        roleName={roleName}
        userName={user?.userName}
        onLogout={handleLogout}
      />

      <main className="min-h-screen pt-16 lg:pt-0 lg:ml-72">
        <div className="p-4 sm:p-6 lg:p-8">
          <Outlet context={{ roleName }} />
        </div>
      </main>
    </div>
  );
}

export default MainLayout;
