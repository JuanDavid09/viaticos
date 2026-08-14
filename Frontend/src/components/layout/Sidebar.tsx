import { NavLink } from "react-router-dom";
import { FileText, FolderOpen, Home, Inbox } from "lucide-react";
import { navItems } from "@/app/routes";
import { env } from "@/config/env";

const icons = {
  "/": Home,
  "/legalizaciones": FileText,
  "/bandejas": Inbox,
  "/soportes": FolderOpen,
} as const;

export function Sidebar() {
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
        {navItems.map((item) => {
          const Icon = icons[item.to];
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
        Fase 0 — Fundación visual
        {env.apiBaseUrl ? ` · API ${env.apiBaseUrl}` : " · API vía proxy Vite"}
      </div>
    </aside>
  );
}
