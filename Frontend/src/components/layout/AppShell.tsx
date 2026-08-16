import { useState } from "react";
import { Menu, X } from "lucide-react";
import { Outlet } from "react-router-dom";
import { Sidebar } from "@/components/layout/Sidebar";

export function AppShell() {
  const [navOpen, setNavOpen] = useState(false);

  function closeNav() {
    setNavOpen(false);
  }

  return (
    <div className={`app-shell${navOpen ? " nav-open" : ""}`}>
      <header className="mobile-nav-bar">
        <button
          type="button"
          className="btn btn-ghost mobile-nav-toggle"
          onClick={() => setNavOpen((open) => !open)}
          aria-expanded={navOpen}
          aria-controls="app-sidebar"
        >
          {navOpen ? <X size={20} /> : <Menu size={20} />}
          Menú
        </button>
        <strong>Viáticos</strong>
      </header>

      {navOpen ? (
        <button
          type="button"
          className="sidebar-backdrop"
          aria-label="Cerrar menú"
          onClick={closeNav}
        />
      ) : null}

      <Sidebar id="app-sidebar" onNavigate={closeNav} />
      <div className="main">
        <Outlet />
      </div>
    </div>
  );
}
