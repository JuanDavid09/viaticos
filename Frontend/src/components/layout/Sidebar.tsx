import { NavLink } from "react-router-dom";
import { BarChart3, FileText, FolderOpen, Home, Inbox, Users } from "lucide-react";
import { getNavItemsForRole } from "@/app/routes";
import { AppLogo } from "@/components/branding/AppLogo";
import { useAuth } from "@/features/auth/AuthContext";
import { env } from "@/config/env";

const icons: Record<string, typeof Home> = {
  "/": Home,
  "/legalizaciones": FileText,
  "/bandejas": Inbox,
  "/reportes": BarChart3,
  "/soportes": FolderOpen,
  "/usuarios": Users,
};

type SidebarProps = {
  id?: string;
  onNavigate?: () => void;
};

export function Sidebar({ id, onNavigate }: SidebarProps) {
  const { session } = useAuth();
  const visibleItems = session ? getNavItemsForRole(session.rol) : [];

  return (
    <aside className="sidebar" id={id}>
      <AppLogo variant="sidebar" />

      <nav className="nav-group" aria-label="Principal">
        <div className="nav-label">Navegación</div>
        {visibleItems.map((item) => {
          const Icon = icons[item.to] ?? Home;
          return (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.to === "/"}
              className={({ isActive }) => `nav-link${isActive ? " active" : ""}`}
              onClick={onNavigate}
            >
              <Icon size={18} />
              {item.label}
            </NavLink>
          );
        })}
      </nav>

      <div className="sidebar-footer">
        {env.appName}
      </div>
    </aside>
  );
}
