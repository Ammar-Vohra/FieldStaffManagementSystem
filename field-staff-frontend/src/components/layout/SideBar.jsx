
import { useState } from "react";
import { NavLink } from "react-router-dom";

function MenuItem({ menu }) {
  const [expanded, setExpanded] = useState(false);

  const hasChildren = menu.children?.length > 0;

  if (!menu.isActive) return null;

  return (
    <div>
      {hasChildren ? (
        <>
          <button
            onClick={() => setExpanded(!expanded)}
            className="w-full flex items-center justify-between px-4 py-3 rounded-lg text-slate-300 hover:bg-slate-700 hover:text-white transition"
          >
            <span>{menu.menuName}</span>

            <span>{expanded ? "−" : "+"}</span>
          </button>

          {expanded && (
            <div className="ml-4 mt-1 border-l border-slate-600 pl-2">
              {menu.children.map((child) => (
                <MenuItem key={child.menuId} menu={child} />
              ))}
            </div>
          )}
        </>
      ) : menu.menuPath ? (
        <NavLink
          to={menu.menuPath}
          className={({ isActive }) =>
            `block px-4 py-3 rounded-lg transition ${
              isActive
                ? "bg-blue-600 text-white"
                : "text-slate-300 hover:bg-slate-700 hover:text-white"
            }`
          }
        >
          {menu.menuName}
        </NavLink>
      ) : null}
    </div>
  );
}

function Sidebar({ menus, roleName, userName, onLogout }) {
  return (
    <aside className="w-64 min-h-screen bg-slate-900 text-white flex flex-col">

      {/* Application Title */}
      <div className="p-5 border-b border-slate-700">
        <h2 className="text-lg font-bold">
          Field Staff Management
        </h2>

        <p className="text-sm text-slate-400 mt-1">
          {roleName || "User"} Panel
        </p>
      </div>

      {/* User Information */}
      <div className="p-4 border-b border-slate-700">
        <p className="text-sm text-slate-400">Logged in as</p>
        <p className="font-medium">{userName}</p>
      </div>

      {/* Dynamic Menus */}
      <nav className="flex-1 p-3 space-y-1">
        {menus.map((menu) => (
          <MenuItem key={menu.menuId} menu={menu} />
        ))}
      </nav>

      {/* Logout */}
      <div className="p-4 border-t border-slate-700">
        <button
          onClick={onLogout}
          className="w-full text-left px-4 py-3 rounded-lg text-red-400 hover:bg-slate-700 transition"
        >
          Logout
        </button>
      </div>

    </aside>
  );
}

export default Sidebar;
