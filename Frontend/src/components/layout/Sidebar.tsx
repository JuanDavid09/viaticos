import { NavLink } from "react-router-dom";
import { FileText, FolderOpen, Home, Inbox, Users } from "lucide-react";
import { getNavItemsForRole } from "@/app/routes";
import { useAuth } from "@/features/auth/AuthContext";
import { env } from "@/config/env";

const icons: Record<string, typeof Home> = {
  "/": Home,
  "/legalizaciones": FileText,
  "/bandejas": Inbox,
  "/soportes": FolderOpen,
  "/usuarios": Users,
};

export function Sidebar() {
  const { session } = useAuth();
  const visibleItems = session ? getNavItemsForRole(session.rol) : [];

  return (
    <aside className="sidebar">
      <div className="brand">
        <div className="brand-mark">V</div>
        <div>
          <h1>Viáticos</h1>
          <p>Legalización interna</p>
        </div>
      </div>

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
            >
              <Icon size={18} />
              {item.label}
            </NavLink>
          );
        })}
      </nav>

      <div className="sidebar-footer">
        Fase 3 — Workflow y bandejas
        {env.apiBaseUrl ? ` · API ${env.apiBaseUrl}` : " · API vía proxy Vite"}
      </div>
    </aside>
  );
}
