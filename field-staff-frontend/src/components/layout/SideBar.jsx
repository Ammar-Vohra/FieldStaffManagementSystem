
import { useState } from "react";
import { NavLink } from "react-router-dom";
import {
  LayoutDashboard,
  Users,
  ClipboardList,
  Building2,
  ChartColumn,
  Settings,
  User,
  Shield,
  KeyRound,
  ClipboardCheck,
  Flag,
  ListChecks,
  Contact,
  ChartBar,
  UsersRound,
  PanelLeft,
  UserRoundCog,
  CheckSquare,
  CircleDot,
  LogOut,
  Menu,
  X,
} from "lucide-react";

const ICON_MAP = {
  "layout-dashboard": LayoutDashboard,
  "users": Users,
  "clipboard-list": ClipboardList,
  "building-2": Building2,
  "chart-column": ChartColumn,
  "settings": Settings,
  "user": User,
  "shield": Shield,
  "key-round": KeyRound,
  "clipboard-check": ClipboardCheck,
  "flag": Flag,
  "list-checks": ListChecks,
  "contact": Contact,
  "chart-bar": ChartBar,
  "users-round": UsersRound,
  "panel-left": PanelLeft,
  "user-round-cog": UserRoundCog,
  "bi-check2-square": CheckSquare,
};

function MenuIcon({ name }) {
  const IconComponent = ICON_MAP[name] || CircleDot;

  return <IconComponent className="w-4 h-4 shrink-0" />;
}

function MenuItem({ menu, onNavigate }) {
  const [expanded, setExpanded] = useState(false);

  const hasChildren = menu.children?.length > 0;

  if (!menu.isActive) return null;

  return (
    <div>
      {hasChildren ? (
        <>
          <button
            type="button"
            onClick={() => setExpanded(!expanded)}
            className="w-full flex items-center justify-between gap-2 px-4 py-3 rounded-lg text-slate-300 hover:bg-slate-700 hover:text-white transition"
          >
            <span className="flex items-center gap-3 min-w-0 text-left">
              <MenuIcon name={menu.icon} />
              <span className="leading-snug">{menu.menuName}</span>
            </span>

            <span className="shrink-0 text-slate-400">
              {expanded ? "−" : "+"}
            </span>
          </button>

          {expanded && (
            <div className="ml-4 mt-1 border-l border-slate-600 pl-2 space-y-1">
              {menu.children.map((child) => (
                <MenuItem
                  key={child.menuId}
                  menu={child}
                  onNavigate={onNavigate}
                />
              ))}
            </div>
          )}
        </>
      ) : menu.menuPath ? (
        <NavLink
          to={menu.menuPath}
          onClick={onNavigate}
          className={({ isActive }) =>
            `flex items-center gap-3 px-4 py-3 rounded-lg transition ${
              isActive
                ? "bg-blue-600 text-white"
                : "text-slate-300 hover:bg-slate-700 hover:text-white"
            }`
          }
        >
          <MenuIcon name={menu.icon} />
          <span className="leading-snug">{menu.menuName}</span>
        </NavLink>
      ) : null}
    </div>
  );
}

function Sidebar({ menus = [], roleName, userName, onLogout }) {
  const [isOpen, setIsOpen] = useState(false);

  const closeSidebar = () => setIsOpen(false);

  return (
    <>
      {/* Mobile Header */}
      <div className="lg:hidden fixed top-0 left-0 right-0 z-40 h-16 bg-slate-900 text-white flex items-center justify-between px-4 shadow-md">
        <div>
          <h2 className="font-bold text-base">
            Field Staff Management
          </h2>
          <p className="text-xs text-slate-400">
            {roleName || "User"} Panel
          </p>
        </div>

        <button
          type="button"
          onClick={() => setIsOpen(!isOpen)}
          className="p-2 rounded-lg hover:bg-slate-700 transition"
          aria-label={isOpen ? "Close menu" : "Open menu"}
        >
          {isOpen ? (
            <X className="w-6 h-6" />
          ) : (
            <Menu className="w-6 h-6" />
          )}
        </button>
      </div>

      {/* Mobile Overlay */}
      {isOpen && (
        <div
          onClick={closeSidebar}
          className="lg:hidden fixed inset-0 z-40 bg-black/50"
        />
      )}

      {/* Sidebar */}
      <aside
        className={`
          fixed top-0 left-0 z-50
          h-screen w-72 max-w-[85vw]
          bg-slate-900 text-white
          flex flex-col
          transition-transform duration-300 ease-in-out
          ${isOpen ? "translate-x-0" : "-translate-x-full"}
          lg:translate-x-0
        `}
      >
        {/* Application Title */}
        <div className="shrink-0 p-5 border-b border-slate-700 flex items-center justify-between">
          <div>
            <h2 className="text-lg font-bold">
              Field Staff Management
            </h2>

            <p className="text-sm text-slate-400 mt-1">
              {roleName || "User"} Panel
            </p>
          </div>

          <button
            type="button"
            onClick={closeSidebar}
            className="lg:hidden p-1 rounded hover:bg-slate-700"
            aria-label="Close sidebar"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* User Information */}
        <div className="shrink-0 p-4 border-b border-slate-700">
          <p className="text-sm text-slate-400">
            Logged in as
          </p>

          <p className="font-medium truncate">
            {userName}
          </p>
        </div>

        {/* Dynamic Menus */}
        <nav className="flex-1 min-h-0 overflow-y-auto scrollbar-hide p-3 space-y-1.5">
          {menus.map((menu) => (
            <MenuItem
              key={menu.menuId}
              menu={menu}
              onNavigate={closeSidebar}
            />
          ))}
        </nav>

        {/* Logout */}
        <div className="shrink-0 p-4 border-t border-slate-700">
          <button
            type="button"
            onClick={onLogout}
            className="w-full flex items-center gap-3 px-4 py-3 rounded-lg text-red-400 hover:bg-slate-700 hover:text-red-300 transition"
          >
            <LogOut className="w-4 h-4 shrink-0" />
            <span>Logout</span>
          </button>
        </div>
      </aside>
    </>
  );
}

export default Sidebar;
